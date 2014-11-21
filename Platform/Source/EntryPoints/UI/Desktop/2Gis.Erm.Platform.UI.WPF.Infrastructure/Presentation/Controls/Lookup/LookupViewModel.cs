using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Input;

using DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Messages;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;
using DoubleGis.Platform.UI.WPF.Infrastructure.MVVM;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Lookup
{
    public sealed class LookupViewModel : ViewModelBase, IViewModel
    {
        private readonly IViewModelIdentity _identity = new OrdinaryViewModelIdentity();
        private readonly IMessageSink _messageSink;
        private readonly LookupPropertyFeature _settings;
        private readonly Guid _hostingViewModelId;

        private LookupEntry _selectedItem;
        private ObservableCollection<LookupEntry> _items;
        
        private bool _isBusy;
        private ICommand _navigateCommand;
        private ICommand _searchCommand;
        private ICommand _doubleClickCommand;
        
        private string _searchText;
        
        public LookupViewModel(IMessageSink messageSink, LookupPropertyFeature settings, Guid hostingViewModelId)
        {
            _messageSink = messageSink;
            _settings = settings;
            _hostingViewModelId = hostingViewModelId;

            Items = new ObservableCollection<LookupEntry>();
            Items.CollectionChanged += ItemsOnCollectionChanged;

            SearchCommand = new DelegateCommand<string>(ExecuteSearch);
            NavigateCommand = new DelegateCommand(ExecuteNavigation);
            DoubleClickCommand = new DelegateCommand(OnDoubleClick);
        }

        public IViewModelIdentity Identity 
        {
            get
            {
                return _identity;
            }
        }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (value == _searchText)
                {
                    return;
                }
                _searchText = value;
                RaisePropertyChanged();
               // RaisePropertyChanged(() => State);
            }
        }

        public ObservableCollection<LookupEntry> Items
        {
            get { return _items; }
            set
            {
                if (Equals(value, _items))
                {
                    return;
                }
                _items = value;
                RaisePropertyChanged();
                RaisePropertyChanged(() => State);
            }
        }

        public ICommand DoubleClickCommand
        {
            get { return _doubleClickCommand; }
            set
            {
                if (Equals(value, _doubleClickCommand))
                {
                    return;
                }
                _doubleClickCommand = value;
                RaisePropertyChanged();
            }
        }

        public ICommand SearchCommand
        {
            get { return _searchCommand; }
            set
            {
                if (Equals(value, _searchCommand))
                {
                    return;
                }
                _searchCommand = value;
                RaisePropertyChanged();
            }
        }

        public ICommand NavigateCommand
        {
            get { return _navigateCommand; }
            set
            {
                if (Equals(value, _navigateCommand))
                {
                    return;
                }
                _navigateCommand = value;
                RaisePropertyChanged();
            }
        }

        public LookupEntry SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (Equals(value, _selectedItem))
                {
                    return;
                }

                _selectedItem = value;
                OnSelectedItemChanged();
                RaisePropertyChanged();
            }
        }

        public LookupState State
        {
            get
            {
                if (Items.Count == 0)
                {
                    return string.IsNullOrWhiteSpace(SearchText) ? LookupState.Empty : LookupState.Failure;
                }

                return SelectedItem != null ? LookupState.SingleResult : LookupState.MultipleResult;
            }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if (value.Equals(_isBusy))
                {
                    return;
                }
                _isBusy = value;
                RaisePropertyChanged();
            }
        }
        
        private void OnSelectedItemChanged()
        {
            if (SelectedItem != null)
            {
                SearchText = SelectedItem.Value;
                RaisePropertyChanged(() => State);
            }
        }
        
        private void ItemsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            SelectedItem = Items.Count == 1 ? Items[0] : null;

            RaisePropertyChanged(() => State);
        }

        private void ExecuteNavigation()
        {
            MessageBox.Show("Navigation command");
        }

        private void ExecuteSearch(string s)
        {
            _messageSink.Post(new LookupSearchMessage(s, _settings, _hostingViewModelId));
        }

        private void OnDoubleClick()
        {
            Items.Clear();
            SelectedItem = null;
            SearchText = null;
            RaisePropertyChanged(() => State);
        }
    }
}
