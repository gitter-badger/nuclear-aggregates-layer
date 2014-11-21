using System.Collections.Generic;
using System.Windows.Controls;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.ContextualNavigation
{
    public sealed class ContextualNavigationConfig : IContextualNavigationConfig
    {
        private readonly INavigationItem[] _navigationItems;

        public ContextualNavigationConfig(INavigationItem[] navigationItems)
        {
            _navigationItems = navigationItems;
        }

        bool IViewModelAspect.Enabled
        {
            get
            {
                return true;
            }
        }

        public INavigationItem[] Items
        {
            get
            {
                return _navigationItems;
            }
        }

        public IReadOnlyDictionary<string, INavigationItem> Parts { get; set; }
        public DataTemplateSelector ReferecedItemsViewsSelector { get; set; }
    }
}