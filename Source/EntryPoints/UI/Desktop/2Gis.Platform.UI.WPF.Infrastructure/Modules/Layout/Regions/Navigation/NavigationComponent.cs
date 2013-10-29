using System.Windows.Controls;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Components;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation
{
    /// <summary>
    /// Компонент для панели навигации Shell
    /// </summary>
    public sealed class NavigationComponent<TViewModel, TView>
        : InstanceIndependentLayoutComponent<ILayoutNavigationComponent, INavigationArea, TViewModel, TView>, ILayoutNavigationComponent
        where TView : Control
        where TViewModel : class, INavigationArea
    {
    }
}
