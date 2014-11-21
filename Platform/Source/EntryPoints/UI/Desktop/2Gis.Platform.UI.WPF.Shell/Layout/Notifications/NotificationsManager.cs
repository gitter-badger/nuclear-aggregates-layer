using System.Linq;
using System.Windows.Controls;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Components;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Notifications;
using DoubleGis.Platform.UI.WPF.Infrastructure.MVVM;
using DoubleGis.Platform.UI.WPF.Infrastructure.Util;

namespace DoubleGis.Platform.UI.WPF.Shell.Layout.Notifications
{
    public sealed class NotificationsManager : ViewModelBase, INotificationsManagerViewModel
    {
        private readonly INotificationList _notificationList;

        public NotificationsManager(INotificationList notificationsList, ILayoutComponentsRegistry layoutComponentsRegistry)
        {
            _notificationList = notificationsList;

            var layoutNotificationsComponent = 
                layoutComponentsRegistry.GetComponentsForLayoutRegion<ILayoutNotificationsComponent>().Single();

            ViewSelector = new ComponentSelector<ILayoutNotificationsComponent, INotificationList>(new[] { layoutNotificationsComponent });
        }

        public INotificationList NotificationList
        {
            get
            {
                return _notificationList;
            }
        }

        public DataTemplateSelector ViewSelector { get; private set; }
    }
}