using System.Collections.Generic;
using System.Windows.Controls;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.ContextualNavigation
{
    public interface IContextualNavigationViewModel
    {
        IReadOnlyDictionary<string, INavigationItem> Parts { get; }
        object ReferencedItemContext { get; set; }
        DataTemplateSelector ReferencedItemViewSelector { get; }
        INavigationItem[] Items { get; }
        ITitleProvider Title { get; }
    }

    public interface IContextualNavigationViewModel<TViewModel> : IContextualNavigationViewModel
        where TViewModel : class, IViewModel
    {
    }
}
