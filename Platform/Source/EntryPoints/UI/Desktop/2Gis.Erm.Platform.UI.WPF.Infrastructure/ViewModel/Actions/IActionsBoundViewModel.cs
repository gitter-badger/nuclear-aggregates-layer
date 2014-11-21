using System.Collections.Generic;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Actions
{
    public interface IActionsBoundViewModel
    {
        IEnumerable<INavigationItem> Actions { get; }
    }

    public interface IActionsBoundViewModel<TViewModel> : IActionsBoundViewModel
        where TViewModel : class, IViewModel
    {
    }
}
