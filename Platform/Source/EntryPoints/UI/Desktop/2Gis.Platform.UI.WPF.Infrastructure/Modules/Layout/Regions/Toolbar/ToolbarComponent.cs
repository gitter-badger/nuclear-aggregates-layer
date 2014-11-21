using System.Windows.Controls;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Components;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Toolbar
{
    /// <summary>
    /// Компонент для региона действий (он же toolbar) Shell
    /// </summary>
    public sealed class ToolbarComponent<TViewModel, TView>
        : InstanceIndependentLayoutComponent<ILayoutToolbarComponent, IToolbar, TViewModel, TView>, ILayoutToolbarComponent
        where TView : Control 
        where TViewModel : class, IToolbar
    {
    }
}
