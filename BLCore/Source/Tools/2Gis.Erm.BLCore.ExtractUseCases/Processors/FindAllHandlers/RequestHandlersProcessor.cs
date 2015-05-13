using System;
using System.Collections.Concurrent;
using System.Linq;

using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.ExtractUseCases.Utils;

using NuClear.Storage.UseCases;

using Roslyn.Compilers;
using Roslyn.Compilers.Common;
using Roslyn.Compilers.CSharp;
using Roslyn.Services;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindAllHandlers
{
    public class RequestHandlersProcessor : AbstractProcessor
    {
        private readonly Type _iRequestHandlerType;
        private readonly Type _requestHandlerType;

        private readonly ConcurrentDictionary<string, HandlerDescriptor> _request2HandlerMap = new ConcurrentDictionary<string, HandlerDescriptor>();
        private readonly ConcurrentDictionary<string, HandlerDescriptor> _invalidRequest2HandlerMap = new ConcurrentDictionary<string, HandlerDescriptor>();

        public RequestHandlersProcessor(IProcessingContext processingContext, IWorkspace workspace, ISolution solution)
            : base(processingContext, workspace, solution)
        {
            _iRequestHandlerType = typeof(IRequestHandler);
            _requestHandlerType = typeof(RequestHandler<,>);
        }

        #region Overrides of ProcessorBase

        private int _foundRequestHandlers;
        private int _invalidRequestHandlers;

        public override bool IsDocumentProcessingRequired(ISemanticModel semanticModel, IDocument document)
        {
            var classes = document.GetSyntaxRoot().DescendantNodes().OfType<ClassDeclarationSyntax>();
            if (!classes.Any())
            {
                return false;
            }

            return document.SourceCodeKind == SourceCodeKind.Regular;
        }

        public override void ProcessDocument(ISemanticModel semanticModel, IDocument document)
        {
            var classes = document.GetSyntaxRoot().DescendantNodes().OfType<ClassDeclarationSyntax>();
            
            INamedTypeSymbol requestHandlerInterfaceSymbol = semanticModel.Compilation.GetTypeByMetadataName(_iRequestHandlerType.FullName);
            INamedTypeSymbol requestHandlerBaseClassSymbol = semanticModel.Compilation.GetTypeByMetadataName(_requestHandlerType.FullName);

            var excludedTypes = semanticModel.GetAllExcludedTypesCantGetAllInterfaces();

            foreach (var classDeclaration in classes)
            {
                var classSymbol = semanticModel.GetDeclaredSymbolExcludeUnsupported(classDeclaration);
                if (classSymbol == null)
                {
                    continue;
                }

                if (classSymbol.AllInterfaces.Contains(requestHandlerInterfaceSymbol))
                {   // найденный класс реализует RequestHandler
                    if (classSymbol.Equals(requestHandlerBaseClassSymbol))
                    {
                        continue;
                    }

                    var request = GetRequestTypeForHandler(classSymbol, requestHandlerBaseClassSymbol);
                    var requestKey = request.TypeDescritionString();
                    var handlerKey = classSymbol.TypeDescritionString();
                    _request2HandlerMap[requestKey] = new HandlerDescriptor
                        {
                            RequestType = request, 
                            RequestKey = requestKey, 
                            HandlerType = classSymbol, 
                            HandlerKey = handlerKey
                        };
                    ++_foundRequestHandlers;
                }
            }
        }

        private INamedTypeSymbol GetRequestTypeForHandler(INamedTypeSymbol requestHandlerTypeSymbol, INamedTypeSymbol requestHandlerBaseClassSymbol)
        {
            for (var baseType = requestHandlerTypeSymbol.BaseType; baseType != null; baseType = baseType.BaseType)
            {
                if (!baseType.IsGenericType)
                {
                    continue;
                }

                if (!baseType.OriginalDefinition.Equals(requestHandlerBaseClassSymbol))
                {
                    continue;
                }

                var requestTypeSymbol = baseType.TypeArguments[0] as INamedTypeSymbol;
                if (requestTypeSymbol == null)
                {
                    continue;
                }

                // keep generic definitions for generic handlers
                if (requestHandlerTypeSymbol.IsGenericType)
                {
                    return requestTypeSymbol.OriginalDefinition;
                }

                return requestTypeSymbol;
            }

            return null;
        }

        public override void FinishProcessing()
        {
            ProcessingContext.SetValue(RequestHandlerKey.Instance, 
                new RequestHandlerProcessingResults
                {
                    ProcessedHandlers = _request2HandlerMap,
                    InvalidProcessedHandlers = _invalidRequest2HandlerMap,
                    FoundRequestHandlers = _foundRequestHandlers,
                    InvalidRequestHandlers = _invalidRequestHandlers
                });
        }

        #endregion
    }
}
