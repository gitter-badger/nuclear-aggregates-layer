using System;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Notifications
{
    public interface INotification
    {
        Guid SourceId { get; }
        NotificationType NotificationType { get; }
        NotificationLevel NotificationLevel { get; }
        string Description { get; }

        DateTime TimestampUtc { get; }
    }
}