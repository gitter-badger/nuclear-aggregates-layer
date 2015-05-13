using System;
using System.Collections.Concurrent;
using System.Linq;

using DoubleGis.Erm.BLCore.ExtractUseCases.Utils;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers.EntityOperations;

using NuClear.Storage.UseCases;

using Roslyn.Compilers;
using Roslyn.Compilers.Common;
using Roslyn.Compilers.CSharp;
using Roslyn.Services;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindEditableEntityControllers
{
    public class EntityControllersProcessor : AbstractProcessor
    {
        private readonly Type _entityControllerBaseType;

        private readonly ConcurrentDictionary<string, EntityControllerDescriptor> _controllersMap = new ConcurrentDictionary<string, EntityControllerDescriptor>();
        private readonly ConcurrentDictionary<string, EntityControllerDescriptor> _invalidControllersMap = new ConcurrentDictionary<string, EntityControllerDescriptor>();

        public EntityControllersProcessor(IProcessingContext processingContext, IWorkspace workspace, ISolution solution)
            : base(processingContext, workspace, solution)
        {
            _entityControllerBaseType = typeof(CreateOrUpdateController<,>);
        }

        #region Overrides of ProcessorBase

        private int _foundControllers;
        private int _invalidControllers;

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
            
            INamedTypeSymbol entityControllerBaseSymbol = semanticModel.Compilation.GetTypeByMetadataName(_entityControllerBaseType.FullName);

            foreach (var classDeclaration in classes)
            {
                var classSymbol = semanticModel.GetDeclaredSymbolExcludeUnsupported(classDeclaration);
                if (classSymbol == null)
                {
                    continue;
                }

                var entityTypeSymbol = GetControllerEntityTypeHandler(classSymbol, entityControllerBaseSymbol);
                if (entityTypeSymbol == null)
                {
                    continue;
                }

                var entityTypeKey = entityTypeSymbol.TypeDescritionString();
                var controllerKey = classSymbol.TypeDescritionString();
                _controllersMap[controllerKey] = new EntityControllerDescriptor
                    {
                        ControllerType = classSymbol,
                        EntityType = entityTypeSymbol,
                        EntityKey = entityTypeKey,
                        DeclaringDocument = document.Id
                    };
                ++_foundControllers;
            }
        }

        private static INamedTypeSymbol GetControllerEntityTypeHandler(INamedTypeSymbol controllerSymbol, INamedTypeSymbol entityControllerBaseSymbol)
        {
            for (var baseType = controllerSymbol.BaseType; baseType != null; baseType = baseType.BaseType)
            {
                if (!baseType.IsGenericType || !baseType.IsAbstract)
                {
                    continue;
                }

                if (!baseType.OriginalDefinition.Equals(entityControllerBaseSymbol))
                {
                    continue;
                }

                var entityTypeSymbol = baseType.TypeArguments[0] as INamedTypeSymbol;
                if (entityTypeSymbol == null)
                {
                    continue;
                }

                return entityTypeSymbol;
            }

            return null;
        }

        public override void FinishProcessing()
        {
            ProcessingContext.SetValue(EntityControllerProcessingResultsKey.Instance, 
                new EntityControllerProcessingResults
                {
                    ProcessedControllers = _controllersMap,
                    InvalidProcessedControllers = _invalidControllersMap,
                    FoundControllers = _foundControllers,
                    InvalidControllers = _invalidControllers
                });
        }

        #endregion
    }
}

