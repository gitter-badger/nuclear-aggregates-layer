using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Handlers;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging.Concrete;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Handlers
{
    public class NotificationFeedbackHandler : UseCaseSyncMessageHandlerBase<NotificationFeedbackMessage>
    {
        protected override IMessageProcessingResult ConcreteHandle(NotificationFeedbackMessage message, IUseCase useCase)
        {
            var currentElement = useCase.State.Current;
            if (currentElement == null)
            {
                return null;
            }

            var viewModel = currentElement.Context as IViewModel;
            if (viewModel == null)
            {
                return null;
            }

            IViewModelWithFeedback viewModelWithFeedback;
            if (viewModel.TryGetElement(
                vm => vm.Identity.Id == message.TargetId && vm is IViewModelWithFeedback, 
                out viewModelWithFeedback))
            {
                viewModelWithFeedback.ActivateNotificationSource(message.NotificationListEntry);
                return EmptyResult;
            }

            return null;
        }
    }
}