using System;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Notifications;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Messaging.Concrete
{
    public class NotificationFeedbackMessage : MessageBase<FreeProcessingModel>
    {
        public NotificationFeedbackMessage(Guid targetId, INotification notificationListEntry)
            : base(null)
        {
            TargetId = targetId;
            NotificationListEntry = notificationListEntry;
        }

        public Guid TargetId { get; private set; }
        public INotification NotificationListEntry { get; private set; }
    }
}