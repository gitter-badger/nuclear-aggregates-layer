using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

using DoubleGis.Erm.BLCore.UI.WPF.Client.DI.UseCase.ViewModel.Aspects;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Documents.ViewModels;
using DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Cards;
using DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Documents;
using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Handler.Concrete;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Utils;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.DI.UseCase.ViewModel
{
    public sealed class UnityCardDocumentViewModelFactory : ICardDocumentViewModelFactory
    {
        private delegate bool DocumentElementResolver(
            IUseCase useCase,
            EntityName entityName, 
            long entityId, 
            IDocumentElementStructure elementStructure, 
            ICollection<IViewModel> viewModels,
            ICollection<IViewModelViewMapping> viewModelsModelViewMappings);

        private readonly IDocumentStructuresProvider _documentStructuresProvider;
        private readonly ICardStructuresProvider _cardStructuresProvider;
        private readonly ICardViewModelFactory _cardViewModelFactory;
        private readonly IGridViewModelFactory _gridViewModelFactory;
        private readonly IViewModelAspectResolver[] _aspectResolvers;
        private readonly IEnumerable<DocumentElementResolver> _elementsResolvers;

        public UnityCardDocumentViewModelFactory(
            IDocumentStructuresProvider documentStructuresProvider,
            ICardStructuresProvider cardStructuresProvider,
            ICardViewModelFactory cardViewModelFactory,
            IGridViewModelFactory gridViewModelFactory,
            IViewModelAspectResolver[] aspectResolvers)
        {
            _documentStructuresProvider = documentStructuresProvider;
            _cardStructuresProvider = cardStructuresProvider;
            _cardViewModelFactory = cardViewModelFactory;
            _gridViewModelFactory = gridViewModelFactory;
            _aspectResolvers = aspectResolvers;
            _elementsResolvers = new DocumentElementResolver[] { ReferencedCard, AttachedGrid };
        }

        public TViewModel Create<TViewModel>(IUseCase useCase, EntityName entityName, long entityId) where TViewModel : class, IViewModel
        {
            return (TViewModel)Create(useCase, entityName, entityId);
        }

        public IViewModel Create(IUseCase useCase, EntityName entityName, long entityId)
        {
            IViewModel documentViewModel;
            if (TryCreateCompositeCardDocument(useCase, entityName, entityId, out documentViewModel))
            {
                return documentViewModel;
            }

            return CreateOrdinaryCardDocument(useCase, entityName, entityId);
        }

        private IViewModel CreateOrdinaryCardDocument(IUseCase useCase, EntityName entityName, long entityId)
        {
            var cardViewModel = _cardViewModelFactory.Create(useCase, entityName, entityId);

            CardStructure cardStructure;
            if (!_cardStructuresProvider.TryGetDescriptor(entityName, out cardStructure) || cardStructure.ViewModelViewMapping == null)
            {
                throw new InvalidOperationException("Can't resolve view type by card structure for entity " + entityName);
            }

            var container = useCase.ResolveFactoryContext();
            var resolvedDependencies = new List<ResolverOverride>();

            var viewModelIdentity = new CardViewModelIdentity
                {
                    EntityId = entityId,
                    EntityName = entityName,
                    OperationIdentity = entityName.ResolveTargetModelOperation<ModifyBusinessModelEntityIdentity, ModifySimplifiedModelEntityIdentity>()
                };

            if (cardStructure.ElementFeatures != null && cardStructure.ElementFeatures.Any())
            {
                foreach (var resolver in _aspectResolvers)
                {
                    DependencyOverride resolvedDependency;
                    if (resolver.TryResolveDependency(useCase, cardStructure, viewModelIdentity, out resolvedDependency))
                    {
                        resolvedDependencies.Add(resolvedDependency);
                    }
                }
            }

            resolvedDependencies.Add(new DependencyOverride(typeof(ICardViewModelIdentity), viewModelIdentity));
            resolvedDependencies.Add(new DependencyOverride(typeof(IEnumerable<IViewModel>),  new IViewModel[] { cardViewModel }));
            resolvedDependencies.Add(new DependencyOverride(typeof(DataTemplateSelector), new ViewModel2ViewMappingsSelector(new[] { cardStructure.ViewModelViewMapping })));

            return container.Resolve<CompositeDocumentViewModel>(resolvedDependencies.ToArray());
        }

        private bool TryCreateCompositeCardDocument(IUseCase useCase, EntityName entityName, long entityId, out IViewModel documentViewModel)
        {
            documentViewModel = null;

            foreach (var documentStructure in _documentStructuresProvider.Structures)
            {
                var documentElement = documentStructure.Elements.FirstOrDefault();
                if (documentElement == null)
                {
                    continue;
                }

                var referenceElement = documentElement as IReferencedElementStructure;
                if (referenceElement == null)
                {
                    continue;
                }

                var cardStructureIdentity = referenceElement.ReferencedElementIdentity as ICardStructureIdentity;
                if (cardStructureIdentity == null || cardStructureIdentity.EntityName != entityName)
                {
                    continue;
                }

                if (documentStructure.ViewModelViewMapping == null || documentStructure.ViewModelViewMapping.ViewModelType == null)
                {
                    throw new InvalidOperationException("Invalid structure for card document dependencies. MVVM parts is not configured properly. EntityName: " + entityName);
                }
                
                var resolvedElements = new List<IViewModel>();
                var viewModelViewMappings = new List<IViewModelViewMapping>();
                foreach (var element in documentStructure.DocumentElements)
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
                if (documentStructure.ElementFeatures != null && documentStructure.ElementFeatures.Any())
                {
                    foreach (var resolver in _aspectResolvers)
                    {
                        DependencyOverride resolvedDependency;
                        if (resolver.TryResolveDependency(useCase, documentStructure, viewModelIdentity, out resolvedDependency))
                        {
                            resolvedDependencies.Add(resolvedDependency);
                        }
                    }
                }

                resolvedDependencies.Add(new DependencyOverride(typeof(IEnumerable<IViewModel>), resolvedElements));
                resolvedDependencies.Add(new DependencyOverride(typeof(DataTemplateSelector), new ViewModel2ViewMappingsSelector(viewModelViewMappings)));

                documentViewModel = (IViewModel)container.Resolve(documentStructure.ViewModelViewMapping.ViewModelType, resolvedDependencies.ToArray());
                return true;
            }

            return false;
        }

        private void ResolveDocumentElement(
            IUseCase useCase, 
            EntityName entityName, 
            long entityId, 
            IDocumentElementStructure elementStructure,
            ICollection<IViewModel> viewModels,
            ICollection<IViewModelViewMapping> viewModelsModelViewMappings)
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
            EntityName entityName, 
            long entityId, 
            IDocumentElementStructure elementStructure,
            ICollection<IViewModel> viewModels,
            ICollection<IViewModelViewMapping> viewModelsModelViewMappings)
        {
            var referenceElement = elementStructure as IReferencedElementStructure;
            if (referenceElement == null)
            {
                return false;
            }

            var cardStructure = referenceElement.ReferencedElement as CardStructure;
            if (cardStructure == null)
            {
                return false;
            }

            viewModels.Add(_cardViewModelFactory.Create(useCase, entityName, entityId));
            viewModelsModelViewMappings.Add(cardStructure.ViewModelViewMapping);

            return true;
        }

        private bool AttachedGrid(
            IUseCase useCase, 
            EntityName entityName, 
            long entityId, 
            IDocumentElementStructure elementStructure,
            ICollection<IViewModel> viewModels,
            ICollection<IViewModelViewMapping> viewModelsModelViewMappings)
        {
            var attachedElement = elementStructure as IAttachedElementStructure;
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
            viewModelsModelViewMappings.Add(attachedElement.ViewModelViewMapping);

            return true;
        }
    }
}
