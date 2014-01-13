using System.Linq;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;
using DoubleGis.Platform.UI.WPF.Infrastructure.MVVM;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Navigation.ViewModels
{
    public sealed class OrdinaryNavigationArea : ViewModelBase, INavigationArea
    {
        private readonly int _id;
        private readonly ITitleProvider _titleProvider;
        private readonly IViewModelIdentity _identity = new OrdinaryViewModelIdentity();
        
        public OrdinaryNavigationArea(int id, ITitleProvider titleProvider)
        {
            _id = id;
            _titleProvider = titleProvider;
        }

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public IViewModelIdentity Identity
        {
            get
            {
                return _identity;
            }
        }

        public string Title 
        {
            get
            {
                return _titleProvider.Title;
            }
        }

        public IImageProvider Icon { get; set; }

        public string Description { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (_isSelected != value)
                {
                    if (value)
                    {
                        SyncNavigationState();
                    }

                    _isSelected = value;
                    RaisePropertyChanged(() => IsSelected);
                }
            }
        }

        private INavigationItem[] _items;
        public bool IsActive
        {
            get
            {
                return true;
            }
        }

        public INavigationItem[] Items
        {
            get
            {
                return _items;
            }
            set
            {
                if (_items != value)
                {
                    if (value != null)
                    {
                        value.ProcessAll(item => item.IsExpanded = true);
                        value.Do(item => item.Items == null || !item.Items.Any(), false, ApplySelection);
                    }

                    _items = value;
                    RaisePropertyChanged(() => Items);
                }
            }
        }

        private void ApplySelection(INavigationItem item)
        {
            item.IsSelected = true;
        }

        private void SyncNavigationState()
        {
            var items = _items;
            if (items == null)
            {
                return;
            }

            items.Do(item => 
                (item.Items == null 
                || !item.Items.Any())
                && item.IsSelected, false, Navigate);
        }

        private void Navigate(INavigationItem item)
        {
            item.NavigateCommand.Execute(item);
        }
    }
}