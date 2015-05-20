using System.Linq;

using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Documents.ViewModels;
using DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Messages;
using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Features.ViewModelViewMap;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Handlers;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.ContextualNavigation;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Provider;
using NuClear.Metamodeling.UI.Elements.Aspects.Features.Handler.Concrete;
using NuClear.Metamodeling.UI.Elements.Concrete.Hierarchy;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Handlers
{
    public sealed class NavigationRelatedItemMessageHandler : UseCaseSyncMessageHandlerBase<NavigationRelatedItemMessage>
    {
        private readonly IMetadataProvider _metadataProvider;
        private readonly IGridViewModelFactory _gridViewModelFactory;

        public NavigationRelatedItemMessageHandler(
            IMetadataProvider metadataProvider,
            IGridViewModelFactory gridViewModelFactory)
        {
            _metadataProvider = metadataProvider;
            _gridViewModelFactory = gridViewModelFactory;
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

            var metadataId = NuClear.Metamodeling.Elements.Identities.Builder.Metadata.Id.For<MetadataCardsIdentity>(cardViewModelWithIdentity.ConcreteIdentity.EntityName.Description);
            CardMetadata cardMetadata;
            if (!_metadataProvider.TryGetMetadata(metadataId, out cardMetadata))
            {
                return null;
            }


            if (!cardMetadata.HasRelatedItems)
            {
                return null;
            }

            IMetadataElement targetMetadataElement;
            if (!cardMetadata.RelatedItems.TryGetElementById(message.UsedItem.Id, out targetMetadataElement))
            {
                return null;
            }

            var hierarchyElement = targetMetadataElement as OldUIElementMetadata;
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

        private object GetRelatedItemViewModel(IUseCase useCase, OldUIElementMetadata metadata)
        {
            var mappingFeature = metadata.Features.OfType<IViewModelViewMappingFeature>().SingleOrDefault();
            if (mappingFeature == null)
            {
                return null;
            }

            if (!metadata.HasHandler)
            {
                return null;
            }

            var showGridDetail = metadata.Handler as ShowGridHandlerFeature;
            if (showGridDetail == null)
            {
                return null;
            }

            return _gridViewModelFactory.Create(useCase, showGridDetail.EntityName);
        }
    }
}
