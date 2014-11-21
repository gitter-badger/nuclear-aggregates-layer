using System.Collections.Generic;

using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Actions
{
    public interface IActionsContainer : IViewModelAspect
    {
        IEnumerable<INavigationItem> Actions { get; }
    }
}
