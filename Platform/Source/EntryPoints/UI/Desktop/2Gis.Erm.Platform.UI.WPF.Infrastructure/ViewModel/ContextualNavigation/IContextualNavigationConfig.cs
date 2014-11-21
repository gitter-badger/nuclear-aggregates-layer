using System.Collections.Generic;
using System.Windows.Controls;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.ContextualNavigation
{
    public interface IContextualNavigationConfig : IViewModelAspect
    {
        INavigationItem[] Items { get; }
        IReadOnlyDictionary<string, INavigationItem> Parts { get; }
        DataTemplateSelector ReferecedItemsViewsSelector { get; }
    }
}
