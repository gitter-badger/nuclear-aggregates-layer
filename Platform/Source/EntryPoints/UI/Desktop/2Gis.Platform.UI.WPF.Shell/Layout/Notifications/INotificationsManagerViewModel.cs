using System.Windows.Controls;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Notifications;

namespace DoubleGis.Platform.UI.WPF.Shell.Layout.Notifications
{
    public interface INotificationsManagerViewModel
    {
        INotificationList NotificationList { get; }
        DataTemplateSelector ViewSelector { get; }
    }
}