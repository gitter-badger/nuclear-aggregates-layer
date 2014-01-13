using System.Linq;

using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Documents.ViewModels;
using DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Cards;
using DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Messages;
using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card;
using DoubleGis.Erm.Platform.Model.Metadata.Common;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Handler.Concrete;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Hierarchy;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Handlers;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.ContextualNavigation;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Handlers
{
    public sealed class NavigationRelatedItemMessageHandler : UseCaseSyncMessageHandlerBase<NavigationRelatedItemMessage>
    {
        private readonly IGridViewModelFactory _gridViewModelFactory;
        private readonly ICardStructuresProvider _cardStructuresProvider;

        public NavigationRelatedItemMessageHandler(IGridViewModelFactory gridViewModelFactory, ICardStructuresProvider cardStructuresProvider)
        {
            _gridViewModelFactory = gridViewModelFactory;
            _cardStructuresProvider = cardStructuresProvider;
        }

        protected override bool ConcreteCanHandle(NavigationRelatedItemMessage message, IUseCase useCase)
        {
            return !useCase.State.IsEmpty;
        }

        protected override IMessageProcessingResult ConcreteHandle(NavigationRelatedItemMessage message, IUseCase useCase)
        {
            var cardViewModel = ResolveCurrentCardViewModel(useCase);
            if (cardViewModel == null)
            {
                return null;
            }

            var contextualNavigationViewModel = cardViewModel as IContextualNavigationViewModel;
            if (contextualNavigationViewModel == null)
            {
                return null;
            }

            var cardViewModelWithIdentity = cardViewModel as ICardViewModel<ICardViewModelIdentity>;
            if (cardViewModelWithIdentity == null)
            {
                return null;
            }

            CardStructure cardStructure;
            if (!_cardStructuresProvider.TryGetDescriptor(cardViewModelWithIdentity.ConcreteIdentity.EntityName, out cardStructure))
            {
                return null;
            }


            if (!cardStructure.HasRelatedItems)
            {
                return null;
            }

            IConfigElement targetConfigElement;
            if (!cardStructure.RelatedItems.TryGetElementById(message.UsedItem.Id, out targetConfigElement))
            {
                return null;
            }

            var hierarchyElement = targetConfigElement as HierarchyElement;
            contextualNavigationViewModel.ReferencedItemContext = GetRelatedItemViewModel(useCase, hierarchyElement);

            return EmptyResult;
        }

        private ICardViewModel ResolveCurrentCardViewModel(IUseCase useCase)
        {
            var currentElement = useCase.State.Current;
            if (currentElement == null)
            {
                return null;
            }

            var cardViewModel = currentElement.Context as ICardViewModel;
            if (cardViewModel != null)
            {
                return cardViewModel;
            }

            var compositeDocument = currentElement.Context as ICompositeDocumentViewModel;
            if (compositeDocument == null)
            {
                return null;
            }

            return compositeDocument.ComposedViewModels.FirstOrDefault() as ICardViewModel;
        }

        private object GetRelatedItemViewModel(IUseCase useCase, HierarchyElement element)
        {
            var mappingFeature = element.ElementFeatures.OfType<IViewModelViewMappingFeature>().SingleOrDefault();
            if (mappingFeature == null)
            {
                return null;
            }

            if (!element.HasHandler)
            {
                return null;
            }

            var showGridDetail = element.Handler as ShowGridHandlerFeature;
            if (showGridDetail == null)
            {
                return null;
            }

            return _gridViewModelFactory.Create(useCase, showGridDetail.EntityName);
        }
    }
}
