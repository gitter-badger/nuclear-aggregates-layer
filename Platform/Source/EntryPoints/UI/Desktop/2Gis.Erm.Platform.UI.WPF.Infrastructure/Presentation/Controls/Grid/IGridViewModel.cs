using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid.Filter;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid.Pager;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid.ViewSelector;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Toolbar;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Messages;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;
using DoubleGis.Platform.UI.WPF.Infrastructure.MVVM;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid
{
    public interface IGridViewModel : 
        IViewModel, 
        IMessageSource<EntitySelectedMessage>
    {
        IPagerViewModel Pager { get; }
        IFilterViewModel Filter { get; }
        IListSelectorViewModel ViewSelector { get; }
        IToolbarViewModel Toolbar { get; }

        DataViewViewModel CurrentView { get; }

        DelegateCommand<NavigationDescriptor> NavigateCommand { get; }
        DelegateCommand<object> ShowEntryDetailCommand { get; }
        DelegateCommand<SortingDescriptor> SortCommand { get; }

        bool IsBusy { get; }
        object[] ListingStorage { get; }
        object[] SelectedItems { get; set; }
    }

    public interface IGridViewModel<TGridViewModelIdentity> : IViewModel<TGridViewModelIdentity>, IGridViewModel
        where TGridViewModelIdentity : class, IGridViewModelIdentity
    {
    }
}
