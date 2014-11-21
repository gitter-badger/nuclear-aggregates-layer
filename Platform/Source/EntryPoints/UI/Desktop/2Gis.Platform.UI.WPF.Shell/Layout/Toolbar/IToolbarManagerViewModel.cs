using System.Windows.Controls;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Toolbar;

namespace DoubleGis.Platform.UI.WPF.Shell.Layout.Toolbar
{
    public interface IToolbarManagerViewModel
    {
        IToolbar Toolbar { get; }
        DataTemplateSelector ViewSelector { get; }
    }
}
