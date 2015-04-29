using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindAllRequests;
using DoubleGis.Erm.BLCore.ExtractUseCases.Utils;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

using Roslyn.Compilers;
using Roslyn.Compilers.Common;
using Roslyn.Compilers.CSharp;
using Roslyn.Services;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindAllSubRequestCalls
{
    public class ExplicitSubRequestsProcessor : AbstractProcessor
    {
        private readonly ConcurrentDictionary<string, ICollection<SubRequestDescriptor>> _handler2SubRequestsMap
            = new ConcurrentDictionary<string, ICollection<SubRequestDescriptor>>();
        private readonly ConcurrentDictionary<string, ICollection<SubRequestDescriptor>> _invalidHandler2SubRequestsMap
            = new ConcurrentDictionary<string, ICollection<SubRequestDescriptor>>();

        private readonly Type _subRequestProcessorType;
        private readonly Type _requestBaseType;
        private INamedTypeSymbol _cachedSubRequestProcessorInterfaceSymbol;
        private INamedTypeSymbol _cachedRequestBaseSymbol;

        public ExplicitSubRequestsProcessor(IProcessingContext processingContext, IWorkspace workspace, ISolution solution)
            : base(processingContext, workspace, solution)
        {
            _subRequestProcessorType = typeof(ISubRequestProcessor);
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

            var subRequestProcessorInterfaceSymbol = semanticModel.Compilation.GetTypeByMetadataName(_subRequestProcessorType.FullName);
            if (subRequestProcessorInterfaceSymbol == null)
            {
                // ISubRequestProcessor не определен в документе => он в нем не используется и нет смысла проверять такой документ
                return false;
            }

            var requestBaseSymbol = semanticModel.Compilation.GetTypeByMetadataName(_requestBaseType.FullName);
            if (requestBaseSymbol == null)
            {
                // Request не определен в документе => он в нем не используется и нет смысла проверять такой документ
                return false;
            }

            _cachedSubRequestProcessorInterfaceSymbol = subRequestProcessorInterfaceSymbol;
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
                if (!IsInvocationCallForTargetMethod(semanticModel, invocation))
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
                        TypeDescriptor = new RequestTypeDescriptor { RequestKey = requestKey, RequestType = requestTypeSymbol },
                        ContainingType = containingClass
                    };

                var handlerKey = containingClass.TypeDescritionString();
                if (IsValidRequestType(requestTypeSymbol))
                {
                    _handler2SubRequestsMap.AddOrUpdate(
                        handlerKey,
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
                    // подходы к обработке более хитрых случаев инстанцирования requests
                    //if (containingClass.Name.Contains("ExportLocalMessageHandler") && containingClassMember.Name.Contains("ProcessExportMessage"))
                    //{
                    //    var references = requestSymbol.FindReferences(Solution);
                    //    foreach (var referencedSymbol in references)
                    //    {
                    //        foreach (var location in referencedSymbol.Locations)
                    //        {
                    //            var docText = location.Document.GetText();
                    //            var token = location.Document.GetSyntaxRoot().FindToken(location.Location.SourceSpan.Start);
                    //            var syntax = token.Parent as SyntaxNode;
                    //            //var syntax = token as CommonSyntaxNode;
                    //            var symbolInfo = semanticModel.GetSymbolInfo(syntax);
                    //            var declaredInfo = semanticModel.GetDeclaredSymbol(syntax);
                    //            var typeInfo = semanticModel.GetTypeInfo(syntax);
                    //            var info = semanticModel.GetSymbolFromAnyNodeType(syntax);
                    //            var spec1 = semanticModel.GetSpeculativeSymbolInfo(token.Span.Start, syntax, SpeculativeBindingOption.BindAsTypeOrNamespace);
                    //            var spec2 = semanticModel.GetSpeculativeSymbolInfo(token.Span.Start, syntax, SpeculativeBindingOption.BindAsExpression);
                    //            var reference = docText.GetSubText(location.Location.SourceSpan);
                    //        }
                    //    }

                    //    var method = containingElement as BaseMethodDeclarationSyntax;

                    //    var mdf = semanticModel.AnalyzeDataFlow(method.DescendantNodes().OfType<BlockSyntax>().FirstOrDefault());
                    //    var mcf = semanticModel.AnalyzeControlFlow(method.DescendantNodes().OfType<BlockSyntax>().FirstOrDefault());
                    //    var rdf = semanticModel.AnalyzeDataFlow(requestSyntax);
                    //}
                    
                    _invalidHandler2SubRequestsMap.AddOrUpdate(
                        handlerKey,
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

        private bool IsInvocationCallForTargetMethod(ISemanticModel semanticModel, InvocationExpressionSyntax invocation)
        {
            if (invocation == null || !invocation.ArgumentList.Arguments.Any())
            {
                return false;
            }

            var call = invocation.DescendantNodes().OfType<MemberAccessExpressionSyntax>().FirstOrDefault();
            if (call == null)
            {
                return false;
            }

            const string TargetMethodName = "HandleSubRequest";

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

            if (!callSymbol.ConstructedFrom.ContainingType.Equals(_cachedSubRequestProcessorInterfaceSymbol))
            {
                return false;
            }


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
            ProcessingContext.SetValue(SubRequestsProcessingResultKey.Instance,
                new SubRequestsProcessingResult
                    {
                        SubRequests = _handler2SubRequestsMap,
                        InvalidProcessedSubRequests = _invalidHandler2SubRequestsMap,
                        FoundSubRequests = _foundSubRequests,
                        InvalidSubRequests = _invalidSubRequests
                    });
        }

        #endregion
    }
}
