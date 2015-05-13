using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.ExtractUseCases.Utils;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

using NuClear.Storage.UseCases;

using Roslyn.Compilers;
using Roslyn.Compilers.Common;
using Roslyn.Compilers.CSharp;
using Roslyn.Services;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindPublicServiceUseCases
{
    public class UseCaseStartEndpointProcessor : AbstractProcessor
    {
        private readonly ConcurrentDictionary<string, ICollection<UseCaseEndpointDescriptor>> _request2EndpointsMap 
            = new ConcurrentDictionary<string, ICollection<UseCaseEndpointDescriptor>>();
        private readonly ConcurrentDictionary<string, ICollection<UseCaseEndpointDescriptor>> _invalidRequest2EndpointsMap
            = new ConcurrentDictionary<string, ICollection<UseCaseEndpointDescriptor>>();

        private readonly Type _publicServiceType;
        private readonly Type _requestBaseType;
        private INamedTypeSymbol _cachedPublicServiceInterfaceSymbol;
        private INamedTypeSymbol _cachedRequestBaseSymbol;
        private int _foundUseCases;
        private int _invalidUseCases;

        public UseCaseStartEndpointProcessor(IProcessingContext processingContext, IWorkspace workspace, ISolution solution)
            : base(processingContext, workspace, solution)
        {
            _publicServiceType = typeof(IPublicService);
            _requestBaseType = typeof(Request);
        }

        #region Overrides of ProcessorBase
        public override bool IsDocumentProcessingRequired(ISemanticModel semanticModel, IDocument document)
        {
            if (document.SourceCodeKind != SourceCodeKind.Regular)
            {
                return false;
            }

            var classes = document.GetSyntaxRoot().DescendantNodes().OfType<ClassDeclarationSyntax>();
            if (!classes.Any())
            {
                return false;
            }

            var publicServiceInterfaceSymbol = semanticModel.Compilation.GetTypeByMetadataName(_publicServiceType.FullName);
            if (publicServiceInterfaceSymbol == null)
            {
                // IPublicService не определен в документе => он в нем не используется и нет смысла проверять такой документ
                return false;
            }

            var requestBaseSymbol = semanticModel.Compilation.GetTypeByMetadataName(_requestBaseType.FullName);
            if (requestBaseSymbol == null)
            {
                // Request не определен в документе => он в нем не используется и нет смысла проверять такой документ
                return false;
            }

            _cachedPublicServiceInterfaceSymbol = publicServiceInterfaceSymbol;
            _cachedRequestBaseSymbol = requestBaseSymbol;

            return true;
        }

        public override void ProcessDocument(ISemanticModel semanticModel, IDocument document)
        {
            var classes = document.GetSyntaxRoot().DescendantNodes().OfType<ClassDeclarationSyntax>();
            foreach (var classDeclaration in classes)
            {
                var classSymbol = semanticModel.GetDeclaredSymbolExcludeUnsupported(classDeclaration);
                if (classSymbol == null)
                {
                    continue;
                }

                var supportedClassMembers = classDeclaration.DescendantNodes().Where(node => InvocationsProcessor.SupportedContainingMembers.Contains(node.Kind));
                foreach (var supportedClassMember in supportedClassMembers)
                {
                    var invocationsProcesssor = new InvocationsProcessor(
                        new InvocationsProcessor.InvocationProcessorConfig
                            {
                                Solution = Solution,
                                Document = document,
                                SemanticModel = semanticModel,
                                AddInvocation = ProcessRequest,
                                ContainingClass = classSymbol,
                                ContainingElement = supportedClassMember,
                                TargetMethodContainingSymbol = _cachedPublicServiceInterfaceSymbol,
                                TargetMethodName = "Handle",
                                RequestBaseSymbol = _cachedRequestBaseSymbol
                            });
                    invocationsProcesssor.ProcessInovacations();
                }
            }
        }

        public override void FinishProcessing()
        {
            ProcessingContext.SetValue(UseCaseProcessingResultsKey.Instance, 
                new UseCaseProcessingResults
                {
                    ProcessedUseCases = _request2EndpointsMap,
                    InvalidProcessedUseCases = _invalidRequest2EndpointsMap,
                    FoundUseCases = _foundUseCases,
                    InvalidUseCases = _invalidUseCases
                });
        }

        #endregion
        
        private void ProcessRequest(
            UseCaseEndpointDescriptor useCaseDescriptor,
            bool isValidRequest)
        {
            var requestDescription = useCaseDescriptor.RequestType.TypeDescritionString();
            if (isValidRequest)
            {
                _request2EndpointsMap.AddOrUpdate(
                    requestDescription,
                    symbol => new List<UseCaseEndpointDescriptor> { useCaseDescriptor },
                    (symbol, collection) =>
                {
                    collection.Add(useCaseDescriptor);
                    return collection;
                });
                ++_foundUseCases;
            }
            else
            {
                _invalidRequest2EndpointsMap.AddOrUpdate(
                    requestDescription,
                    symbol => new List<UseCaseEndpointDescriptor> { useCaseDescriptor },
                    (symbol, collection) =>
                {
                    collection.Add(useCaseDescriptor);
                    return collection;
                });
                ++_invalidUseCases;
            }
        }
    }
}
