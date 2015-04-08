using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.WPF.Client.DI.UseCase.ViewModel.Aspects;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Documents.ViewModels;
using DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Documents;
using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.UI.Elements.Aspects.Features.Handler.Concrete;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Provider;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Utils;

using Microsoft.Practices.Unity;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.DI.UseCase.ViewModel
{
    public sealed class UnityCardDocumentViewModelFactory : ICardDocumentViewModelFactory
    {
        private delegate bool DocumentElementResolver(
            IUseCase useCase,
            IEntityType entityName,
            long entityId,
            IMetadataElement elementMetadata,
            ICollection<IViewModel> viewModels,
            ICollection<IViewModelViewTypeMapping> viewModelsModelViewMappings);

        private readonly IMetadataProvider _metadataProvider;
        private readonly ICardViewModelFactory _cardViewModelFactory;
        private readonly IGridViewModelFactory _gridViewModelFactory;
        private readonly IViewModelAspectResolver[] _aspectResolvers;
        private readonly IEnumerable<DocumentElementResolver> _elementsResolvers;

        public UnityCardDocumentViewModelFactory(
            IMetadataProvider metadataProvider,
            ICardViewModelFactory cardViewModelFactory,
            IGridViewModelFactory gridViewModelFactory,
            IViewModelAspectResolver[] aspectResolvers)
        {
            _metadataProvider = metadataProvider;
            _cardViewModelFactory = cardViewModelFactory;
            _gridViewModelFactory = gridViewModelFactory;
            _aspectResolvers = aspectResolvers;
            _elementsResolvers = new DocumentElementResolver[] { ReferencedCard, AttachedGrid };
        }

        public TViewModel Create<TViewModel>(IUseCase useCase, IEntityType entityName, long entityId) where TViewModel : class, IViewModel
        {
            return (TViewModel)Create(useCase, entityName, entityId);
        }

        public IViewModel Create(IUseCase useCase, IEntityType entityName, long entityId)
        {
            IViewModel documentViewModel;
            if (TryCreateCompositeCardDocument(useCase, entityName, entityId, out documentViewModel))
            {
                return documentViewModel;
            }

            return CreateOrdinaryCardDocument(useCase, entityName, entityId);
        }

        private IViewModel CreateOrdinaryCardDocument(IUseCase useCase, IEntityType entityName, long entityId)
        {
            var cardViewModel = _cardViewModelFactory.Create(useCase, entityName, entityId);

            var metadataId = NuClear.Metamodeling.Elements.Identities.Builder.Metadata.Id.For<MetadataCardsIdentity>(entityName.ToString());
            CardMetadata cardMetadata;
            if (!_metadataProvider.TryGetMetadata(metadataId, out cardMetadata))
            {
                throw new InvalidOperationException("Can't resolve view type by card structure for entity " + entityName);
            }

            if (cardMetadata.ViewModelViewMapping == null)
            {
                throw new InvalidOperationException("Invalid metadata config for card " + entityName);
            }

            var container = useCase.ResolveFactoryContext();
            var resolvedDependencies = new List<ResolverOverride>();

            var viewModelIdentity = new CardViewModelIdentity
                {
                    EntityId = entityId,
                    EntityName = entityName,
                    OperationIdentity = entityName.ResolveTargetModelOperation<ModifyBusinessModelEntityIdentity, ModifySimplifiedModelEntityIdentity>()
                };

            if (cardMetadata.Features != null && cardMetadata.Features.Any())
            {
                foreach (var resolver in _aspectResolvers)
                {
                    DependencyOverride resolvedDependency;
                    if (resolver.TryResolveDependency(useCase, cardMetadata, viewModelIdentity, out resolvedDependency))
                    {
                        resolvedDependencies.Add(resolvedDependency);
                    }
                }
            }

            resolvedDependencies.Add(new DependencyOverride(typeof(ICardViewModelIdentity), viewModelIdentity));
            resolvedDependencies.Add(new DependencyOverride(typeof(IEnumerable<IViewModel>),  new IViewModel[] { cardViewModel }));
            resolvedDependencies.Add(new DependencyOverride(typeof(DataTemplateSelector), new ViewModel2ViewMappingsSelector(new[] { (IViewModelViewTypeMapping)cardMetadata.ViewModelViewMapping })));

            return container.Resolve<CompositeDocumentViewModel>(resolvedDependencies.ToArray());
        }

        private bool TryCreateCompositeCardDocument(IUseCase useCase, IEntityType entityName, long entityId, out IViewModel documentViewModel)
        {
            documentViewModel = null;

            MetadataSet documentsMetadata;
            if (!_metadataProvider.TryGetMetadata<MetadataDocumentsIdentity>(out documentsMetadata))
            {
                throw new InvalidOperationException("Can't resolve documents metadata");
            }

            foreach (var documentMetadata in documentsMetadata.Metadata.Values.OfType<DocumentMetadata>())
            {
                var cardMetadataElements = documentMetadata.Elements<CardMetadata>().Where(cm => cm.Entity == entityName);
                if (!cardMetadataElements.Any())
                {
                    continue;
                }

                if (documentMetadata.ViewModelViewMapping == null || documentMetadata.ViewModelViewMapping.ViewModelType == null)
                {
                    throw new InvalidOperationException("Invalid structure for card document dependencies. MVVM parts is not configured properly. EntityName: " + entityName);
                }
                
                var resolvedElements = new List<IViewModel>();
                var viewModelViewMappings = new List<IViewModelViewTypeMapping>();
                foreach (var element in documentMetadata.Elements)
                {
                    ResolveDocumentElement(useCase, entityName, entityId, element, resolvedElements, viewModelViewMappings);
                }
                
                var container = useCase.ResolveFactoryContext();

                var viewModelIdentity = new CardViewModelIdentity
                {
                    EntityId = entityId,
                    EntityName = entityName,
                    OperationIdentity = entityName.ResolveTargetModelOperation<ModifyBusinessModelEntityIdentity, ModifySimplifiedModelEntityIdentity>()
                };

                var resolvedDependencies = new List<ResolverOverride>();
                if (documentMetadata.Features != null && documentMetadata.Features.Any())
                {
                    foreach (var resolver in _aspectResolvers)
                    {
                        DependencyOverride resolvedDependency;
                        if (resolver.TryResolveDependency(useCase, documentMetadata, viewModelIdentity, out resolvedDependency))
                        {
                            resolvedDependencies.Add(resolvedDependency);
                        }
                    }
                }

                resolvedDependencies.Add(new DependencyOverride(typeof(IEnumerable<IViewModel>), resolvedElements));
                resolvedDependencies.Add(new DependencyOverride(typeof(DataTemplateSelector), new ViewModel2ViewMappingsSelector(viewModelViewMappings)));

                documentViewModel = (IViewModel)container.Resolve(documentMetadata.ViewModelViewMapping.ViewModelType, resolvedDependencies.ToArray());
                return true;
            }

            return false;
        }

        private void ResolveDocumentElement(
            IUseCase useCase, 
            IEntityType entityName, 
            long entityId,
            IMetadataElement elementStructure,
            ICollection<IViewModel> viewModels,
            ICollection<IViewModelViewTypeMapping> viewModelsModelViewMappings)
        {
            if (_elementsResolvers.Any(
                documentElementResolver => documentElementResolver(useCase, entityName, entityId, elementStructure, viewModels, viewModelsModelViewMappings)))
            {
                return;
            }

            throw new InvalidOperationException("Can't resolve document element. " + elementStructure);
        }

        private bool ReferencedCard(
            IUseCase useCase, 
            IEntityType entityName, 
            long entityId,
            IMetadataElement elementMetadata,
            ICollection<IViewModel> viewModels,
            ICollection<IViewModelViewTypeMapping> viewModelsModelViewMappings)
        {
            var cardMetadata = elementMetadata as CardMetadata;
            if (cardMetadata == null)
            {
                return false;
            }

            viewModels.Add(_cardViewModelFactory.Create(useCase, entityName, entityId));
            viewModelsModelViewMappings.Add((IViewModelViewTypeMapping)cardMetadata.ViewModelViewMapping);

            return true;
        }

        private bool AttachedGrid(
            IUseCase useCase, 
            IEntityType entityName, 
            long entityId,
            IMetadataElement elementMetadata,
            ICollection<IViewModel> viewModels,
            ICollection<IViewModelViewTypeMapping> viewModelsModelViewMappings)
        {
            var attachedElement = elementMetadata as AttachedMetadata;
            if (attachedElement == null || !attachedElement.HasHandler)
            {
                return false;
            }

            var showGridHandlerFeature = attachedElement.Handler as ShowGridHandlerFeature;
            if (showGridHandlerFeature == null)
            {
                return false;
            }

            var gridViewModel = _gridViewModelFactory.Create(useCase, showGridHandlerFeature.EntityName);
            viewModels.Add(gridViewModel);
            viewModelsModelViewMappings.Add((IViewModelViewTypeMapping)attachedElement.ViewModelViewMapping);

            return true;
        }
    }
}
