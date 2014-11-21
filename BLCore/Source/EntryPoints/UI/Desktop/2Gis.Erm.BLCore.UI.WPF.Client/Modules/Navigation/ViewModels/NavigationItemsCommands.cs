using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;
using DoubleGis.Platform.UI.WPF.Infrastructure.MVVM;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Navigation.ViewModels
{
    public static class NavigationItemsCommands
    {
        public static DelegateCommand<INavigationItem> DoNothing
        {
            get
            {
                return new DelegateCommand<INavigationItem>(DoNothingExecute, FakeCanExecute);
            }
        }

        private static bool FakeCanExecute(INavigationItem item)
        {
            return true;
        }

        private static void DoNothingExecute(INavigationItem item)
        {
            // do nothing
        }
    }
}