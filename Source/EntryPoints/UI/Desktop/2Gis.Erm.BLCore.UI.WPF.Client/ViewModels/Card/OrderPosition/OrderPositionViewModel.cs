using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card.OrderPosition.DTOs;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Localization;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Layout.Regions.Documents;
using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.ResourceInfrastructure;
using DoubleGis.Platform.UI.WPF.Infrastructure.MVVM;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card.OrderPosition
{
    public sealed class OrderPositionViewModel : 
        ViewModelBase, 
        ICardViewModel<ICardViewModelIdentity>,
        ITitledViewModel<OrderPositionViewModel>
    {
        private readonly ICardViewModelIdentity _viewModelIdentity;
        private readonly ITitleProvider _titleProvider;
        private readonly ObservableCollection<OrderPositionAdvertisementDto> _selectedPositions;
        private IEnumerable<OrderPositionAdvertisementViewModel> _firstGeneration;
        
        public OrderPositionViewModel(
            ICardViewModelIdentity viewModelIdentity, 
            ITitleProvider titleProvider)
        {
            _viewModelIdentity = viewModelIdentity;
            _titleProvider = titleProvider;
            _selectedPositions = new ObservableCollection<OrderPositionAdvertisementDto>();
        }

        public IViewModelIdentity Identity
        {
            get { return _viewModelIdentity; }
        }

        public ICardViewModelIdentity ConcreteIdentity
        {
            get { return _viewModelIdentity; }
        }

        public string DocumentName
        {
            get
            {
                return _titleProvider.Title;
            }
        }

        Guid IDocument.Id
        {
            get
            {
                return _viewModelIdentity.Id;
            }
        }
        
        public string Title 
        {
            get { return _titleProvider.Title; }
        }

        public ICollection<OrderPositionAdvertisementDto> SelectedPositions
        {
            get { return _selectedPositions; }
        }

        public IEnumerable<OrderPositionAdvertisementViewModel> FirstGeneration 
        {
            get
            {
                return _firstGeneration;
            }
            set 
            { 

                _firstGeneration = value;
                RaisePropertyChanged();
            }
        }
    }
}
