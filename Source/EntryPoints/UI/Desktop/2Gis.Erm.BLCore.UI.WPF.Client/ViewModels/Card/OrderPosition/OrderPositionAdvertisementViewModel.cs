using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card.OrderPosition.DTOs;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Lookup;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel;
using DoubleGis.Platform.UI.WPF.Infrastructure.MVVM;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card.OrderPosition
{
    public sealed class OrderPositionAdvertisementViewModel : ViewModelBase, IViewModel
    {
        private readonly IViewModelIdentity _identity = new OrdinaryViewModelIdentity();
        private readonly OrderPositionAdvertisementDto _dto;
        private readonly ICollection<OrderPositionAdvertisementDto> _selectedPositions;
        private IList<OrderPositionAdvertisementViewModel> _children;
        private bool _isExpanded;
        private bool _isSelected;
        private LookupViewModel _advertisementLink;

        public OrderPositionAdvertisementViewModel(OrderPositionAdvertisementDto dto, ICollection<OrderPositionAdvertisementDto> selectedPositions)
        {
            _dto = dto;
            _isExpanded = true;
            _selectedPositions = selectedPositions;

            UpdateSelectedPositionsCollection();
        }

        public IViewModelIdentity Identity
        {
            get { return _identity; }
        }

        public ICommand OrderPositionCommand { get; private set; }

        public IList<OrderPositionAdvertisementViewModel> Children
        {
            get
            {
                return _children;
            }

            set
            {
                if (!Equals(value, _children))
                {
                    _children = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }

            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }

            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    RaisePropertyChanged();
                }
            }
        }

        public Visibility CheckVisibility
        {
            get
            {
                if (!_dto.CanBeChecked)
                {
                    return Visibility.Collapsed;
                }

                return _dto.Children != null && _dto.Children.Any()
                           ? Visibility.Collapsed
                           : Visibility.Visible;
            }
        }

        public Visibility AdvertisementVisibility
        {
            get
            {
                if (!_dto.CanBeLinked)
                {
                    return Visibility.Collapsed;
                }

                return _dto.Children != null && _dto.Children.Any()
                           ? Visibility.Collapsed
                           : Visibility.Visible;
            }
        }

        public bool IsLeaf
        {
            get { return _dto.Children == null || !_dto.Children.Any(); }
        }

        public LinkingObjectType LinkingObjectType
        {
            get { return _dto.Type; }
        }

        public string Name
        {
            get
            {
                return _dto.Name;
            }

            set
            {
                if (value != _dto.Name)
                {
                    _dto.Name = value;
                    RaisePropertyChanged();
                }
            }
        }

        public bool IsChecked
        {
            get
            {
                return _dto.IsChecked;
            }

            set
            {
                if (value != _dto.IsChecked)
                {
                    _dto.IsChecked = value;
                    RaisePropertyChanged();
                    UpdateSelectedPositionsCollection();
                }
            }
        }

        public LookupViewModel AdvertisementLink
        {
            get
            {
                return _advertisementLink;
            }

            set
            {
                if (value != _advertisementLink)
                {
                    _advertisementLink = value;
                    RaisePropertyChanged();
                }
            }
        }

        private void UpdateSelectedPositionsCollection()
        {
            if (!IsChecked && _selectedPositions.Contains(_dto))
            {
                _selectedPositions.Remove(_dto);
            }
            else if (IsChecked && !_selectedPositions.Contains(_dto))
            {
                _selectedPositions.Add(_dto);
            }
        }
    }
}
