using System;
using System.Collections.Concurrent;
using System.Linq;

using DoubleGis.Erm.BLCore.ExtractUseCases.Utils;
using DoubleGis.Erm.Platform.API.Core.UseCases.Context;

using NuClear.Model.Common.Entities.Aspects;

using Roslyn.Compilers;
using Roslyn.Compilers.Common;
using Roslyn.Compilers.CSharp;
using Roslyn.Services;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindAllEntities
{
    public class EntitiesProcessor : AbstractProcessor
    {
        private readonly Type _entityMarkerType = typeof(IEntity);
        private readonly Type _domainEntityMarkerType = typeof(IStateTrackingEntity);

        private readonly ConcurrentDictionary<string, EntityDescriptor> _entitiesMap = new ConcurrentDictionary<string, EntityDescriptor>();
        private readonly ConcurrentDictionary<string, EntityDescriptor> _invalidEntitiesMap = new ConcurrentDictionary<string, EntityDescriptor>();

        public EntitiesProcessor(IProcessingContext processingContext, IWorkspace workspace, ISolution solution)
            : base(processingContext, workspace, solution)
        {
        }

        #region Overrides of ProcessorBase

        private int _foundEntities;
        private int _invalidEntities;
        
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

            INamedTypeSymbol entityMarkerSymbol = semanticModel.Compilation.GetTypeByMetadataName(_entityMarkerType.FullName);
            INamedTypeSymbol domainEntityMarkerSymbol = semanticModel.Compilation.GetTypeByMetadataName(_domainEntityMarkerType.FullName);

            foreach (var classDeclaration in classes)
            {
                var classSymbol = semanticModel.GetDeclaredSymbolExcludeUnsupported(classDeclaration);
                if (classSymbol == null)
                {
                    continue;
                }

                if (!IsEntity(classSymbol, entityMarkerSymbol))
                {
                    continue;
                }

                var entityDescription = classSymbol.TypeDescritionString();
                _entitiesMap[entityDescription] = new EntityDescriptor
                    {
                        EntityType = classSymbol,
                        DeclaringDocument = document.Id,
                        IsDomainEntity = IsDomainEntity(classSymbol, domainEntityMarkerSymbol)
                    };

                ++_foundEntities;
            }
        }

        private bool IsEntity(INamedTypeSymbol checkingType, INamedTypeSymbol entityMarkerSymbol)
        {
            return checkingType.AllInterfaces.Contains(entityMarkerSymbol);
        }
        
        private bool IsDomainEntity(INamedTypeSymbol checkingType, INamedTypeSymbol domainEntityMarkerSymbol)
        {
            return checkingType.AllInterfaces.Contains(domainEntityMarkerSymbol);
        }

        public override void FinishProcessing()
        {
            ProcessingContext.SetValue(EntitiesProcessingResultsKey.Instance,
                new EntitiesProcessingResults
                {
                    ProcessedEntities = _entitiesMap,
                    InvalidProcessedEntities = _invalidEntitiesMap,
                    FoundEntities = _foundEntities,
                    InvalidEntities = _invalidEntities
                });
        }

        #endregion
    }
}

