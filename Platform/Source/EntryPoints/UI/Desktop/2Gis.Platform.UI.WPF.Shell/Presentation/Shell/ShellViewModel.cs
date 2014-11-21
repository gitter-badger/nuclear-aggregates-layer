using DoubleGis.Platform.UI.WPF.Infrastructure.MVVM;
using DoubleGis.Platform.UI.WPF.Shell.Layout.Documents;
using DoubleGis.Platform.UI.WPF.Shell.Layout.Navigation;
using DoubleGis.Platform.UI.WPF.Shell.Layout.Notifications;
using DoubleGis.Platform.UI.WPF.Shell.Layout.Toolbar;
using DoubleGis.Platform.UI.WPF.Shell.Layout.UserInfo;

namespace DoubleGis.Platform.UI.WPF.Shell.Presentation.Shell
{
    /// <summary>
    /// UserControl view model.
    /// </summary>
    public sealed class ShellViewModel : ViewModelBase, IShellViewModel
    {
        private readonly INavigationManagerViewModel _navigationManager;
        private readonly IDocumentManagerViewModel _documentManager;
        private readonly IUserManagerViewModel _userManager;
        private readonly IToolbarManagerViewModel _toolbarManager;
        private readonly INotificationsManagerViewModel _notificationsManager;

        public ShellViewModel(INavigationManagerViewModel navigationManager,
                              IDocumentManagerViewModel documentManager,
                              INotificationsManagerViewModel notificationsManager, 
                              IUserManagerViewModel userManager,
                              IToolbarManagerViewModel toolbarManager)
        {
            _navigationManager = navigationManager;
            _documentManager = documentManager;
            _notificationsManager = notificationsManager;
            _userManager = userManager;
            _toolbarManager = toolbarManager;
        }

        public IDocumentManagerViewModel DocumentManager
        {
            get
            {
                return _documentManager;
            }
        }

        public INavigationManagerViewModel NavigationManager
        {
            get
            {
                return _navigationManager;
            }
        }

        public IUserManagerViewModel UserManager
        {
            get
            {
                return _userManager;
            }
        }

        public INotificationsManagerViewModel NotificationsManager
        {
            get
            {
                return _notificationsManager;
            }
        }

        public IToolbarManagerViewModel ToolbarManager
        {
            get
            {
                return _toolbarManager;
            }
        }
    }
}
