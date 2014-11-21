using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Notifications;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Messages
{
    public sealed class NotificationMessage : MessageBase<FreeProcessingModel>
    {
        public NotificationMessage(INotification[] notifications)
            :base(null)
        {
            Notifications = notifications;
        }

        public INotification[] Notifications { get; private set; }
    }
}