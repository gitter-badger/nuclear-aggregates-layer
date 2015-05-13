using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindAllHandlers;
using DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindAllRequests;
using DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindAllSubRequestCalls;
using DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindPublicServiceUseCases;
using DoubleGis.Erm.BLCore.ExtractUseCases.UseCases;
using DoubleGis.Erm.BLCore.ExtractUseCases.Utils;

using NuClear.Storage.UseCases;

using Roslyn.Compilers.Common;
using Roslyn.Services;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Processors.ExtractUseCases
{
    public class ExtractUseCasesProcessor : AbstractProcessor
    {
        public ExtractUseCasesProcessor(IProcessingContext processingContext, IWorkspace workspace, ISolution solution)
            : base(processingContext, workspace, solution)
        {
        }

        #region Overrides of AbstractProcessor

        public override bool IsDocumentProcessingRequired(ISemanticModel semanticModel, IDocument document)
        {
            return false;
        }

        public override void ProcessDocument(ISemanticModel semanticModel, IDocument document)
        {
            throw new NotImplementedException();
        }

        public override void FinishProcessing()
        {
            var requestHandler2RequestMap = ProcessingContext.GetValue(RequestHandlerKey.Instance);
            var useCasesStartEndpoints = ProcessingContext.GetValue(UseCaseProcessingResultsKey.Instance);
            var requests = ProcessingContext.GetValue(RequestsProcessingResultsKey.Instance);

            var processingErrors = 
                new ProcessingErrors(
                    requestHandler2RequestMap.ProcessedHandlers.Keys,
                    requests.ProcessedRequests.Keys);

            var useCases = new List<UseCase>();
            var notMappedHandlers = new List<string>();

            int useCasesMaxDepth = 0;
            foreach (var useCaseCall in useCasesStartEndpoints.ProcessedUseCases)
            {
                foreach (var useCaseInfo in useCaseCall.Value) 
                {   // одинаковыми request могут запускаться несколько usecases (из нескольких разных мест)
                    var appropriateRequestsKeys = GetAppropriateRequestsForCall(useCaseInfo);
                    foreach (var request in appropriateRequestsKeys)
                    {   // одно и то же место запуска usecase по факту может запускать несколько usecase - если используются для вызова какие-то базвые типы requests (например, EditRequest<>)
                        processingErrors.NotUsedRequests.Remove(request.RequestKey);
                        int currentUseCaseMaxDepth = 0;
                        var useCaseRoot = ProcessUseCaseTree(request, processingErrors, notMappedHandlers, 0, ref currentUseCaseMaxDepth);
                        if (useCaseRoot == null)
                        {
                            processingErrors.NotConnectedUseCases.Add(new Tuple<string, UseCaseEndpointDescriptor>(request.RequestKey, useCaseInfo));
                            continue;
                        }

                        useCasesMaxDepth = Math.Max(useCasesMaxDepth, currentUseCaseMaxDepth);

                        var useCase = new UseCase
                            {
                                Description = (!string.IsNullOrEmpty(useCaseInfo.Description) 
                                                    ? useCaseInfo.Description + ". " 
                                                    : string.Empty) 
                                                + useCaseInfo.ContainingClass 
                                                + (useCaseInfo.ContainingClassMember  != null 
                                                    ? ("\\" + useCaseInfo.ContainingClassMember.Name) 
                                                    : string.Empty),
                                Root = useCaseRoot,
                                MaxUseCaseDepth = currentUseCaseMaxDepth
                            };
                        useCases.Add(useCase);
                    }
                }
            }

            ProcessingContext.SetValue(ExtractUseCasesKey.Instance, 
                new ExtractUseCasesProcessingResults
                    {
                        NotMappedHandlers = notMappedHandlers.ToArray(), 
                        ProcessedUseCases = useCases.ToArray(),
                        Errors = processingErrors
                    });
        }

        private IEnumerable<RequestTypeDescriptor> GetAppropriateRequestsForCall(UseCaseEndpointDescriptor endpointDescriptor)
        {
            var callingRequestKey = endpointDescriptor.RequestType.OriginalDefinition.TypeDescritionString();
            var requests = ProcessingContext.GetValue(RequestsProcessingResultsKey.Instance);
            List<RequestDescriptor> descriptors = null;
            if (!requests.ProcessedRequests.TryGetValue(callingRequestKey, out descriptors) || descriptors == null)
            {
                throw new InvalidOperationException("Can't find request descriptors for specified in call request key: " + callingRequestKey);
            }

            var result = new List<RequestTypeDescriptor>();
            foreach (var requestDescriptor in descriptors)
            {
                result.Add(requestDescriptor.TypeDescriptor);
                if (requestDescriptor.TypeVariations.Any())
                {
                    result.AddRange(requestDescriptor.TypeVariations);
                }
            }

            return result;
        }

        private UseCaseNode ProcessUseCaseTree(
            RequestTypeDescriptor request,
            ProcessingErrors processingErrors, 
            ICollection<string> notMappedHandlers, 
            int depthLevel, 
            ref int maxUseCaseDepth)
        {
            var handlerDescriptor = ResolveHandlerForRequest(request, notMappedHandlers);
            if (handlerDescriptor == null)
            {
                return null;
            }

            maxUseCaseDepth = Math.Max(depthLevel, maxUseCaseDepth);

            processingErrors.NotUsedRequests.Remove(request.RequestKey);
            processingErrors.NotCalledHandlers.Remove(request.RequestKey);

            var useCaseNode = new UseCaseNode(depthLevel)
                {
                    ContainingClass = handlerDescriptor.HandlerType,
                    Request = handlerDescriptor.RequestType,
                    RequestKey = handlerDescriptor.RequestKey
                };

            var childNodes = new List<UseCaseNode>();
            var handlersWithSubRequests = ProcessingContext.GetValue(SubRequestsProcessingResultKey.Instance);
            ICollection<SubRequestDescriptor> subRequests = null;
            if (!handlersWithSubRequests.SubRequests.TryGetValue(handlerDescriptor.HandlerKey, out subRequests) || subRequests == null)
            {   // handler не делает подзапросы
                return useCaseNode;
            }

            foreach (var subRequest in subRequests)
            {
                if (childNodes.Any(ch => string.CompareOrdinal(ch.RequestKey, subRequest.TypeDescriptor.RequestKey) == 0))
                {
                    continue;
                }

                var childNode = ProcessUseCaseTree(subRequest.TypeDescriptor, processingErrors, notMappedHandlers, depthLevel + 1, ref maxUseCaseDepth);
                if (childNode != null)
                {
                    childNodes.Add(childNode);
                }
            }

            useCaseNode.ChildNodes = childNodes.ToArray();
            return useCaseNode;
        }

        private HandlerDescriptor ResolveHandlerForRequest(RequestTypeDescriptor request, ICollection<string> notMappedHandlers)
        {
            var requestHandler2RequestMap = ProcessingContext.GetValue(RequestHandlerKey.Instance);
            HandlerDescriptor handler = null;
            if (requestHandler2RequestMap.ProcessedHandlers.TryGetValue(request.RequestKey, out handler) && handler != null)
            {
                return handler;
            }
            
            //var requests = ProcessingContext.GetValue(RequestsProcessingResultsKey.Instance);
            //RequestDescriptor requestDescriptor = null;
            //List<RequestDescriptor> requestDescriptors = null;
            //if (!requests.ProcessedRequests.TryGetValue(request.RequestKey, out requestDescriptors))
            //{
            //    foreach (var entry in requests.ProcessedRequests)
            //    {
            //        var descriptor = entry.Value.First();
            //        if (descriptor.TypeVariations.Any(u => string.CompareOrdinal(u.RequestKey, request.RequestKey) == 0))
            //        {
            //            requestDescriptor = descriptor;
            //            break;
            //        }
            //    }

            //}
            //else
            //{
            //    requestDescriptor = requestDescriptors.First();
            //}

            //if (requestDescriptor == null)
            //{
            //    throw new InvalidOperationException("Can't get request descriptor for key: " + request);
            //}
            //var requestType = requestDescriptor.RequestType;
            var requestType = request.RequestType;
            if (!requestType.IsGenericType)
            {
                return null;
            }

            var genericRequestTypeSymbol = requestType.OriginalDefinition;
            var genericRequestTypeSymbolKey = genericRequestTypeSymbol.TypeDescritionString();
            if (requestHandler2RequestMap.ProcessedHandlers.TryGetValue(genericRequestTypeSymbolKey, out handler) && handler != null)
            {
                return handler;
            }

            notMappedHandlers.Add(request.RequestKey);
            return null;
        }

        #endregion
    }
}
