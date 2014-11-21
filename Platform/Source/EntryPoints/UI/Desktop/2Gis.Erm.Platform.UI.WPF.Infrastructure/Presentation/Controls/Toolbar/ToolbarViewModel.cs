using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;
using DoubleGis.Platform.UI.WPF.Infrastructure.MVVM;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Toolbar
{
    public sealed class ToolbarViewModel : ViewModelBase, IToolbarViewModel
    {
        private readonly ToolbarItemViewSelector _itemsViewSelector = new ToolbarItemViewSelector();
        private IEnumerable<INavigationItem> _items = Enumerable.Empty<INavigationItem>();
        
        public IEnumerable<INavigationItem> Items
        {
            get
            {
                return _items;
            }
            set 
            {
                _items = value ?? Enumerable.Empty<INavigationItem>();
                RaisePropertyChanged();
            }
        }

        public DataTemplateSelector ViewSelector 
        {
            get { return _itemsViewSelector; }
        }

        public bool Enabled
        {
            get { return true; }
        }
    }
}
