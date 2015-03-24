using System;

using DoubleGis.Erm.Platform.Resources.Client;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Localization;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;
using DoubleGis.Platform.UI.WPF.Infrastructure.MVVM;

using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources.Titles;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid.Filter
{
    public sealed class FilterViewModel : ViewModelBase, IFilterViewModel
    {
        private readonly ITitleProvider _filterTitle;
        private DelegateCommand _filterCommand;
        private string _filterText;

        public FilterViewModel(ITitleProviderFactory titleProviderFactory)
        {
            _filterTitle = titleProviderFactory.Create(ResourceTitleDescriptor.Create(() => ResPlatformUI.SearchRecords));
        }
        
        public event Action<string> ApplingFilter;

        public bool Enabled
        {
            get { return true; }
        }
        
        public DelegateCommand FilterCommand
        {
            get { return _filterCommand ?? (_filterCommand = new DelegateCommand(OnFilterCommand, CanExecuteFilterCommand)); }
        }

        public string FilterText
        {
            get
            {
                return _filterText;
            }
            set
            {
                if (_filterText != value)
                {
                    _filterText = value;
                    RaisePropertyChanged();
                    FilterCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public ITitleProvider FilterTitle
        {
            get { return _filterTitle; }
        }
        
        private bool CanExecuteFilterCommand()
        {
            return !string.IsNullOrWhiteSpace(FilterText);
        }

        private void OnFilterCommand()
        {
            FireApplingFilter(FilterText);
        }

        private void FireApplingFilter(string searchText)
        {
            var handler = ApplingFilter;
            if (handler != null)
            {
                handler(searchText);
            }
        }
    }
}
