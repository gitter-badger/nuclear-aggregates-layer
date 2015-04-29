using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindAllHandlers;
using DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindAllRequests;
using DoubleGis.Erm.BLCore.ExtractUseCases.Utils;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.API.Core.UseCases;

using Roslyn.Compilers;
using Roslyn.Compilers.Common;
using Roslyn.Compilers.CSharp;
using Roslyn.Services;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindAllSubRequestCalls
{
    public class ResumedUsecasesProcessor : AbstractProcessor
    {
        private readonly ConcurrentDictionary<string, ICollection<SubRequestDescriptor>> _resumedRequest2SubRequestsMap
            = new ConcurrentDictionary<string, ICollection<SubRequestDescriptor>>();
        private readonly ConcurrentDictionary<string, ICollection<SubRequestDescriptor>> _invalidResumedRequest2SubRequestsMap
            = new ConcurrentDictionary<string, ICollection<SubRequestDescriptor>>();

        private readonly Type _resumeUseCaseInterfaceType;
        private readonly Type _requestBaseType;
        private INamedTypeSymbol _cachedUseCaseResumeInterfaceSymbol;
        private INamedTypeSymbol _cachedRequestBaseSymbol;

        public ResumedUsecasesProcessor(IProcessingContext processingContext, IWorkspace workspace, ISolution solution)
            : base(processingContext, workspace, solution)
        {
            _resumeUseCaseInterfaceType = typeof(IUseCaseResumeContext<>);
            _requestBaseType = typeof(Request);
        }

        #region Overrides of AbstractProcessor
        private int _foundSubRequests;
        private int _invalidSubRequests;

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

            var useCaseResumeInterfaceSymbol = semanticModel.Compilation.GetTypeByMetadataName(_resumeUseCaseInterfaceType.FullName);
            if (useCaseResumeInterfaceSymbol == null)
            {
                // IUseCaseResumeContext не определен в документе => он в нем не используется и нет смысла проверять такой документ
                return false;
            }

            var requestBaseSymbol = semanticModel.Compilation.GetTypeByMetadataName(_requestBaseType.FullName);
            if (requestBaseSymbol == null)
            {
                // Request не определен в документе => он в нем не используется и нет смысла проверять такой документ
                return false;
            }

            _cachedUseCaseResumeInterfaceSymbol = useCaseResumeInterfaceSymbol;
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

                var supportedClassMembers = classDeclaration.DescendantNodes().Where(node => node.Kind == SyntaxKind.FieldDeclaration
                                                                                          || node.Kind == SyntaxKind.MethodDeclaration
                                                                                          || node.Kind == SyntaxKind.ConstructorDeclaration);
                foreach (var supportedClassMember in supportedClassMembers)
                {
                    ISymbol descriptionSymbol = null;
                    switch (supportedClassMember.Kind)
                    {
                        case SyntaxKind.FieldDeclaration:
                        {
                            var field = (FieldDeclarationSyntax)supportedClassMember;
                            descriptionSymbol = semanticModel.GetDeclaredSymbol(field.Declaration.Variables.First());
                            break;
                        }
                        case SyntaxKind.ConstructorDeclaration:
                        case SyntaxKind.MethodDeclaration:
                        {
                            descriptionSymbol = semanticModel.GetDeclaredSymbol(supportedClassMember);
                            break;
                        }
                    }

                    ProcessInovacations(semanticModel, document, supportedClassMember, classSymbol, descriptionSymbol);
                }
            }
        }

        private void ProcessInovacations(
            ISemanticModel semanticModel,
            IDocument document,
            CommonSyntaxNode containingElement,
            INamedTypeSymbol containingClass,
            ISymbol containingClassMember)
        {
            

            var inovacations = containingElement.DescendantNodes().OfType<InvocationExpressionSyntax>();
            foreach (var invocation in inovacations)
            {
                INamedTypeSymbol resumedRequest = null;
                if (!TryGetResumedRequest(semanticModel, invocation, out resumedRequest) || resumedRequest == null)
                {
                    continue;
                }

                var requestSyntax = invocation.ArgumentList.Arguments[0].DescendantNodes().OfType<NameSyntax>().First();
                var requestSymbol = semanticModel.GetSymbolInfo(requestSyntax).Symbol;
                if (requestSymbol == null)
                {
                    throw new InvalidOperationException("Can't find sub request symbol");
                }

                INamedTypeSymbol requestTypeSymbol = ResolveSymbol(requestSymbol);
                if (requestTypeSymbol == null)
                {
                    throw new InvalidOperationException("Not supported type of sub request instance provider. Base symbol: " + requestSymbol);
                }

                var requestKey = requestTypeSymbol.TypeDescritionString();
                var subRequestDescriptor = new SubRequestDescriptor
                {
                    TypeDescriptor = new RequestTypeDescriptor { RequestType = requestTypeSymbol, RequestKey = requestKey },
                    ContainingType = containingClass
                };

                var resumedRequestKey = resumedRequest.TypeDescritionString();
                if (IsValidRequestType(requestTypeSymbol))
                {
                    _resumedRequest2SubRequestsMap.AddOrUpdate(
                        resumedRequestKey,
                        symbol => new List<SubRequestDescriptor> { subRequestDescriptor },
                        (symbol, collection) =>
                    {
                        collection.Add(subRequestDescriptor);
                        return collection;
                    });
                    ++_foundSubRequests;
                }
                else
                {
                    _invalidResumedRequest2SubRequestsMap.AddOrUpdate(
                        resumedRequestKey,
                        symbol => new List<SubRequestDescriptor> { subRequestDescriptor },
                        (symbol, collection) =>
                    {
                        collection.Add(subRequestDescriptor);
                        return collection;
                    });
                    ++_invalidSubRequests;
                }
            }
        }

        private INamedTypeSymbol ResolveSymbol(ISymbol symbol)
        {
            switch (symbol.Kind)
            {
                case CommonSymbolKind.Method:
                {
                    // request создается на new, либо возвращается из вызова метода (фабричного и т.п.)
                    var requestPrepareMethodSymbol = symbol as MethodSymbol;
                    if (requestPrepareMethodSymbol == null)
                    {
                        break;
                    }

                    if (requestPrepareMethodSymbol.MethodKind == MethodKind.Constructor)
                    {
                        return requestPrepareMethodSymbol.ContainingType;
                    }

                    var requestFromMethodType = requestPrepareMethodSymbol.ReturnType as INamedTypeSymbol;
                    if (requestFromMethodType != null)
                    {
                        return requestPrepareMethodSymbol.ContainingType;
                    }
                    break;
                }
                case CommonSymbolKind.Local:
                {
                    var requestContainingLocalSymbol = symbol as ILocalSymbol;
                    if (requestContainingLocalSymbol == null)
                    {
                        break;
                    }

                    return requestContainingLocalSymbol.Type as INamedTypeSymbol;
                }
                case CommonSymbolKind.Parameter:
                {
                    var requestContainingParameterSymbol = symbol as IParameterSymbol;
                    if (requestContainingParameterSymbol == null)
                    {
                        break;
                    }

                    return requestContainingParameterSymbol.Type as INamedTypeSymbol;
                }
            }

            return null;
        }

        private bool TryGetResumedRequest(ISemanticModel semanticModel, InvocationExpressionSyntax invocation, out INamedTypeSymbol resumedRequest)
        {
            resumedRequest = null;

            if (invocation == null || !invocation.ArgumentList.Arguments.Any())
            {
                return false;
            }

            var call = invocation.DescendantNodes().OfType<MemberAccessExpressionSyntax>().FirstOrDefault();
            if (call == null)
            {
                return false;
            }
            
            const string TargetMethodName = "UseCaseResume";

            if (string.Compare(call.DescendantNodes().OfType<IdentifierNameSyntax>().Last().Identifier.ValueText, TargetMethodName, StringComparison.OrdinalIgnoreCase) != 0)
            {
                return false;
            }

            var callSymbol = semanticModel.GetSymbolInfo(call).Symbol as MethodSymbol;
            if (callSymbol == null ||
                callSymbol.Name != TargetMethodName ||
                callSymbol.ConstructedFrom == null)
            {
                return false;
            }

            if (!callSymbol.ConstructedFrom.ContainingType.IsGenericType)
            {
                return false;
            }

            if (!callSymbol.ConstructedFrom.ContainingType.OriginalDefinition.Equals(_cachedUseCaseResumeInterfaceSymbol))
            {
                return false;
            }

            resumedRequest = callSymbol.ConstructedFrom.ContainingType.TypeArguments[0] as INamedTypeSymbol;

            return true;
        }

        private bool IsValidRequestType(INamedTypeSymbol requestTypeSymbol)
        {
            if (requestTypeSymbol.Equals(_cachedRequestBaseSymbol))
            {
                return false;
            }

            for (var baseType = requestTypeSymbol.BaseType; baseType != null; baseType = baseType.BaseType)
            {
                if (baseType.OriginalDefinition.Equals(_cachedRequestBaseSymbol))
                {
                    return true;
                }
            }

            return false;
        }

        public override void FinishProcessing()
        {
            var resultsAlreadyExisting = ProcessingContext.GetValue(SubRequestsProcessingResultKey.Instance);
            
            var subRequests = resultsAlreadyExisting != null
                                  ? new ConcurrentDictionary<string, ICollection<SubRequestDescriptor>>(resultsAlreadyExisting.SubRequests)
                                  : new ConcurrentDictionary<string, ICollection<SubRequestDescriptor>>();

            var invalidProcessedSubRequests = resultsAlreadyExisting != null
                            ? new ConcurrentDictionary<string, ICollection<SubRequestDescriptor>>(resultsAlreadyExisting.InvalidProcessedSubRequests)
                            : new ConcurrentDictionary<string, ICollection<SubRequestDescriptor>>();

            var results = new SubRequestsProcessingResult
                       {
                           SubRequests = subRequests,
                           InvalidProcessedSubRequests = invalidProcessedSubRequests,
                           FoundSubRequests = resultsAlreadyExisting != null ? resultsAlreadyExisting.FoundSubRequests : 0,
                           InvalidSubRequests = resultsAlreadyExisting != null ? resultsAlreadyExisting.InvalidSubRequests : 0
                       };

            var handlers = ProcessingContext.GetValue(RequestHandlerKey.Instance, true);
            foreach (var subRequest in _resumedRequest2SubRequestsMap)
            {
                var resumedRequestKey = subRequest.Key;
                HandlerDescriptor resumedHandlerDescriptor = null;
                if (!handlers.ProcessedHandlers.TryGetValue(resumedRequestKey, out resumedHandlerDescriptor) || resumedHandlerDescriptor == null)
                {
                    throw new InvalidOperationException("Can't find handler for request key: " + resumedRequestKey);
                }

                var calls = subRequest.Value;
                subRequests.AddOrUpdate(
                    resumedHandlerDescriptor.HandlerKey,
                    symbol => new List<SubRequestDescriptor>(calls),
                    (symbol, collection) =>
                    {
                        foreach (var call in calls)
                        {
                            collection.Add(call);
                        }
                           
                        return collection;
                    });
                ++results.FoundSubRequests;
            }

            foreach (var subRequest in _invalidResumedRequest2SubRequestsMap)
            {
                var resumedRequestKey = subRequest.Key;
                HandlerDescriptor resumedHandlerDescriptor = null;
                if (!handlers.ProcessedHandlers.TryGetValue(resumedRequestKey, out resumedHandlerDescriptor) || resumedHandlerDescriptor == null)
                {
                    throw new InvalidOperationException("Can't find handler for request key: " + resumedRequestKey);
                }

                var calls = subRequest.Value;
                invalidProcessedSubRequests.AddOrUpdate(
                    resumedHandlerDescriptor.HandlerKey,
                    symbol => new List<SubRequestDescriptor>(calls),
                    (symbol, collection) =>
                    {
                        foreach (var call in calls)
                        {
                            collection.Add(call);
                        }

                        return collection;
                    });
                ++results.InvalidSubRequests;
            }

            ProcessingContext.SetValue(SubRequestsProcessingResultKey.Instance, results);
        }

        #endregion
    }
}

