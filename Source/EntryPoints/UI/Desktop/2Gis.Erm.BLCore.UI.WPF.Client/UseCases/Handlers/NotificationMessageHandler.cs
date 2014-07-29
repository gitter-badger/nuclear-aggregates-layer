using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Handlers;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Messages;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Notifications;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.UseCases.Handlers
{
    public class NotificationMessageHandler : UseCaseSyncMessageHandlerBase<NotificationMessage>
    {
        private readonly INotificationsManager _notificationsManager;

        public NotificationMessageHandler(INotificationsManager notificationsManager)
        {
            _notificationsManager = notificationsManager;
        }

        protected override IMessageProcessingResult ConcreteHandle(NotificationMessage message, IUseCase useCase)
        {
            _notificationsManager.SetNotifications(message.Notifications);
            return EmptyResult;
        }
    }
}