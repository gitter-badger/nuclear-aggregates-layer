using System.Collections.Generic;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Actions
{
    public sealed class NullActionsContainer : IActionsContainer
    {
        public IEnumerable<INavigationItem> Actions 
        {
            get { return new INavigationItem[0]; }
        }

        bool IViewModelAspect.Enabled
        {
            get
            {
                return false;
            }
        }
    }
}