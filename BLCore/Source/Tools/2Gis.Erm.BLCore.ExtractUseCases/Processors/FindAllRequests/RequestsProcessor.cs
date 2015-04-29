using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindAllHandlers;
using DoubleGis.Erm.BLCore.ExtractUseCases.Utils;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

using Roslyn.Compilers;
using Roslyn.Compilers.Common;
using Roslyn.Compilers.CSharp;
using Roslyn.Services;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindAllRequests
{
    public class RequestsProcessor : AbstractProcessor
    {
        private readonly Type _requestBaseType = typeof(Request);
        
        private readonly ConcurrentDictionary<string, List<RequestDescriptor>> _requestsMap = new ConcurrentDictionary<string, List<RequestDescriptor>>();
        private readonly ConcurrentDictionary<string, List<RequestDescriptor>> _invalidRequestsMap = new ConcurrentDictionary<string, List<RequestDescriptor>>();

        public RequestsProcessor(IProcessingContext processingContext, IWorkspace workspace, ISolution solution)
            : base(processingContext, workspace, solution)
        {
        }

        #region Overrides of ProcessorBase

        private int _foundRequests;
        private int _invalidRequests;
        

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

            INamedTypeSymbol requestBaseClassSymbol = semanticModel.Compilation.GetTypeByMetadataName(_requestBaseType.FullName);
            
            foreach (var classDeclaration in classes)
            {
                var classSymbol = semanticModel.GetDeclaredSymbolExcludeUnsupported(classDeclaration);
                if (classSymbol == null)
                {
                    continue;
                }

                INamedTypeSymbol[] inheritanceChain = null;
                if (!IsValidRequest(classSymbol, requestBaseClassSymbol, out inheritanceChain))
                {
                    continue;
                }

                var requestKey = classSymbol.TypeDescritionString();
                var requestDescriptor = new RequestDescriptor
                    {
                        TypeDescriptor = new RequestTypeDescriptor
                        {
                            RequestKey = requestKey,
                            RequestType = classSymbol,
                        },
                        InheritanceChain = inheritanceChain,
                        DeclaringDocument = document.Id
                    };
                AddToMap(requestDescriptor);
                ++_foundRequests;
            }
        }

        private void AddToMap(RequestDescriptor requestDescriptor)
        {
            // добавляем непосредственно сам конкретный тип request
            _requestsMap.AddOrUpdate(
                requestDescriptor.TypeDescriptor.RequestKey,
                symbol => new List<RequestDescriptor> { requestDescriptor },
                (symbol, collection) =>
                {
                    collection.Insert(0, requestDescriptor);
                    return collection;
                });

            // обрабатываем классы от которых наследует непосредственно сам конкретный тип request
            // т.к. в точке вызова может быть указан не конкертный вызываемый тип request, а базовый класс
            if (requestDescriptor.InheritanceChain.Length > 1)
            {
                for (int i = 0; i < requestDescriptor.InheritanceChain.Length - 1; i++)
                {
                    var parentKey = requestDescriptor.InheritanceChain[i].OriginalDefinition.TypeDescritionString();
                    _requestsMap.AddOrUpdate(
                        parentKey,
                        symbol => new List<RequestDescriptor> { requestDescriptor },
                        (symbol, collection) =>
                        {
                            collection.Add(requestDescriptor);
                            return collection;
                        });
                }
            }
        }

        private bool IsValidRequest(INamedTypeSymbol checkingType, INamedTypeSymbol requestBaseClassSymbol, out INamedTypeSymbol[] inheritanceChain)
        {
            inheritanceChain = null;

            if (checkingType.Equals(requestBaseClassSymbol))
            {
                return false;
            }

            inheritanceChain = GetRequestInheritanceChain(checkingType, requestBaseClassSymbol);

            return inheritanceChain != null && inheritanceChain.Length > 0;
        }

        private INamedTypeSymbol[] GetRequestInheritanceChain(INamedTypeSymbol checkingType, INamedTypeSymbol requestBaseClassSymbol)
        {
            var types = new List<INamedTypeSymbol>();

            for (var baseType = checkingType.BaseType; baseType != null; baseType = baseType.BaseType)
            {
                types.Add(baseType);
                if (baseType.OriginalDefinition.Equals(requestBaseClassSymbol))
                {
                    return types.ToArray();
                }
            }

            return null;
        }

        public override void FinishProcessing()
        {
            var handlers = ProcessingContext.GetValue(RequestHandlerKey.Instance, true);
            foreach (var handler in handlers.ProcessedHandlers)
            {   // обычные requests (не generic)
                if (_requestsMap.ContainsKey(handler.Key))
                {
                    continue;
                }

                // обработка requests типа closed generics, т.е. тип request - общий open generic,
                // а уже для каждого closed generic request есть только один конкретный тип handler
                var genericRequestKey = handler.Value.RequestType.OriginalDefinition.TypeDescritionString();
                List<RequestDescriptor> requests = null;
                if (!_requestsMap.TryGetValue(genericRequestKey, out requests) || requests == null)
                {
                    throw new InvalidOperationException("Can't find appropriate request for handler, searching request key: " + handler.Key);
                }

                requests.First().TypeVariations.Add(
                    new RequestTypeDescriptor
                    {
                        RequestKey = handler.Key, 
                        RequestType = handler.Value.RequestType
                    });
            }

            ProcessingContext.SetValue(RequestsProcessingResultsKey.Instance,
                new RequestsProcessingResults
                {
                    ProcessedRequests = _requestsMap,
                    InvalidProcessedRequests = _invalidRequestsMap,
                    FoundRequests = _foundRequests,
                    InvalidRequests = _invalidRequests
                });
        }

        #endregion
    }
}
