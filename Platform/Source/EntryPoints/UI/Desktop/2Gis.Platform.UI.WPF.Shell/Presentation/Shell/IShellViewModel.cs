using DoubleGis.Platform.UI.WPF.Shell.Layout.Documents;
using DoubleGis.Platform.UI.WPF.Shell.Layout.Navigation;
using DoubleGis.Platform.UI.WPF.Shell.Layout.Notifications;
using DoubleGis.Platform.UI.WPF.Shell.Layout.Toolbar;
using DoubleGis.Platform.UI.WPF.Shell.Layout.UserInfo;

namespace DoubleGis.Platform.UI.WPF.Shell.Presentation.Shell
{
    public interface IShellViewModel
    {
        IDocumentManagerViewModel DocumentManager { get; }
        INavigationManagerViewModel NavigationManager { get; }
        IUserManagerViewModel UserManager { get; }
        INotificationsManagerViewModel NotificationsManager { get; }
        IToolbarManagerViewModel ToolbarManager { get; }
    }
}
