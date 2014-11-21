using System.Collections.Generic;
using System.Windows.Controls;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Toolbar
{
    public sealed class NullToolbarViewModel : IToolbarViewModel
    {
        public bool Enabled
        {
            get { return false; }
        }

        public IEnumerable<INavigationItem> Items { get; set; }
        public DataTemplateSelector ViewSelector { get; private set; }
        
    }
}