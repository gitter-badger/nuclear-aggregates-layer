using System.Collections.Generic;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.ViewModels;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Notifications
{
    public interface INotificationList : ILayoutComponentViewModel
    {
        string ContextualNotificationsTitle { get; }
        string SystemNotificationsTitle { get; }
        string NotificationDescriptionTitle { get; }

        IEnumerable<INotification> ContextualNotifications { get; }
        IEnumerable<INotification> SystemNotifications { get; }
    }
}