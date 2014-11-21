using System.Collections.Generic;
using System.Windows.Controls;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.ViewModels;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Toolbar
{
    public interface IToolbar : ILayoutComponentViewModel
    {
        IEnumerable<INavigationItem> Items { get; set; }
        DataTemplateSelector ViewSelector { get; }
    }
}