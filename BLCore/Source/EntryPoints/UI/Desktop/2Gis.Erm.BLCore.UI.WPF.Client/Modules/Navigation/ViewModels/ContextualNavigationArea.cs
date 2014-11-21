using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Navigation;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;
using DoubleGis.Platform.UI.WPF.Infrastructure.MVVM;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.Modules.Navigation.ViewModels
{
    public sealed class ContextualNavigationArea : ViewModelBase, IContextualNavigationArea
    {
        private readonly IViewModelIdentity _identity = new OrdinaryViewModelIdentity();
        private ITitleProvider _contextSourceTitle;

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
                var titleSource = _contextSourceTitle;
                return titleSource != null ? titleSource.Title : string.Empty;
            }
        }

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
                    _isSelected = value;
                    RaisePropertyChanged(() => IsSelected);
                    RaisePropertyChanged(() => IsActive);
                }
            }
        }

        private INavigationItem[] _items;
        public bool IsActive
        {
            get
            {
                return IsSelected;
            }
        }
        public INavigationItem[] Items
        {
            get
            {
                return _items;
            }
        }

        public void UpdateContext(ITitleProvider contextSourceTitle, INavigationItem[] contextSourceItems)
        {
            _items = contextSourceItems;
            _contextSourceTitle = contextSourceTitle;
            RaisePropertyChanged(() => Items);
            RaisePropertyChanged(() => Title);
        }
    }
}