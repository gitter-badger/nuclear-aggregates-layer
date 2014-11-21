using System.Collections.Generic;
using System.Windows.Controls;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.ContextualNavigation
{
    public sealed class NullContextualNavigationConfig : IContextualNavigationConfig
    {
        bool IViewModelAspect.Enabled
        {
            get
            {
                return false;
            }
        }

        public INavigationItem[] Items
        {
            get
            {
                return new INavigationItem[0];
            }
        }

        public IReadOnlyDictionary<string, INavigationItem> Parts
        {
            get
            {
                return new Dictionary<string, INavigationItem>();
            }
        }
        public DataTemplateSelector ReferecedItemsViewsSelector
        {
            get
            {
                return null;
            }
        }
    }
}