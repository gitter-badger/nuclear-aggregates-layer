using DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Messages;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Handlers;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.ContextualNavigation;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Handlers
{
    public class NavigationShowMainContentMessageHandler : UseCaseSyncMessageHandlerBase<NavigationShowMainContentMessage>
    {
        protected override bool ConcreteCanHandle(NavigationShowMainContentMessage message, IUseCase useCase)
        {
            return !useCase.State.IsEmpty;
        }

        protected override IMessageProcessingResult ConcreteHandle(NavigationShowMainContentMessage message, IUseCase useCase)
        {
            var currentElement = useCase.State.Current;
            if (currentElement == null)
            {
                return null;
            }

            var contextualNavigationViewModel = currentElement.Context as IContextualNavigationViewModel;
            if (contextualNavigationViewModel == null)
            {
                return null;
            }

            contextualNavigationViewModel.ReferencedItemContext = null;
            return EmptyResult;
        }
    }
}
