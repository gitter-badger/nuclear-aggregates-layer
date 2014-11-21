using System.Windows.Controls;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Components;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Notifications
{
    /// <summary>
    /// Компонент для региона списка сообщений Shell
    /// </summary>
    public class NotificationsComponent<TViewModel, TView> : InstanceIndependentLayoutComponent<ILayoutNotificationsComponent, INotificationList, TViewModel, TView>,
                                                             ILayoutNotificationsComponent
        where TView : Control
        where TViewModel : class, INotificationList
    {
    }
}