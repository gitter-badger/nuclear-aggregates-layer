using System.Collections.Generic;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Actions
{
    public sealed class ActionContainer : IActionsContainer
    {
        private readonly IEnumerable<INavigationItem> _actions;

        public ActionContainer(IEnumerable<INavigationItem> navigationItems)
        {
            _actions = navigationItems;
        }

        public IEnumerable<INavigationItem> Actions
        {
            get { return _actions; }
        }

        bool IViewModelAspect.Enabled 
        { 
            get
            {
                return true;
            } 
        }
    }
}
