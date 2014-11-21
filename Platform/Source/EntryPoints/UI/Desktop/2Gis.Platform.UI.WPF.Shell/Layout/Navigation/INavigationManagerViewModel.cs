using System.Collections.Generic;
using System.Windows.Controls;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;

namespace DoubleGis.Platform.UI.WPF.Shell.Layout.Navigation
{
    public interface INavigationManagerViewModel
    {
        IEnumerable<INavigationArea> Areas { get; }
        INavigationArea SelectedArea { get; set; }
        DataTemplateSelector ViewSelector { get; }
    }
}