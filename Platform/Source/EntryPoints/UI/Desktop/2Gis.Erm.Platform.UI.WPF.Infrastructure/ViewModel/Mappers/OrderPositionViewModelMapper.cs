using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Core.Dto.DomainEntity;
using DoubleGis.Erm.Core.Dto.DomainEntity.Custom;
using DoubleGis.Erm.Core.Utils;
using DoubleGis.Erm.Model.Entities;
using DoubleGis.Erm.Model.Entities.Interfaces;
using DoubleGis.Erm.UI.WPF.Client.Common.Model.Metadata.PresentationConfig.Entities.PropertyFeatures;
using DoubleGis.Erm.UI.WPF.Client.Common.Model.Usecase;
using DoubleGis.Erm.UI.WPF.Client.Common.Presentation.Controls.Lookup;
using DoubleGis.Erm.UI.WPF.Client.Common.ViewModel.Card.OrderPosition;
using DoubleGis.UI.WPF.Infrastructure.MVVM;

namespace DoubleGis.Erm.UI.WPF.Client.Common.ViewModel.Mappers
{
    public sealed class OrderPositionViewModelMapper : IViewModelMapper<OrderPositionDomainEntityDto> 
    {
        private readonly ILookupFactory _lookupFactory;

        public OrderPositionViewModelMapper(ILookupFactory lookupFactory)
        {
            _lookupFactory = lookupFactory;
        }

        public OrderPositionDomainEntityDto ToDto(IUseCase useCase, IViewModel sourceViewModel)
        {
            var targetDto = new OrderPositionDomainEntityDto();
            return targetDto;
        }

        public IViewModel FromDto(OrderPositionDomainEntityDto sourceDto, IUseCase useCase, IViewModel targetViewModel)
        {
            var viewModel = (OrderPositionViewModel)targetViewModel;
            var lookupFactory = new LookupViewModelFactory(useCase, targetViewModel.Identity, _lookupFactory);

            // fixme {a.rechkalov, 2013-06-28}: здесь пока кладём на пришедшую от сервера модель, а это не совсем правильно...
            var orderPositionDto = DebugDataProvider.GetOrderPositionDto();
            viewModel.FirstGeneration = CreateOrderPositionAdvertisementViewModel(orderPositionDto.Advertisements, viewModel.SelectedPositions, lookupFactory);

            return targetViewModel;
        }

        IViewModel IViewModelMapper.FromDto(IDomainEntityDto sourceDto, IUseCase useCase, IViewModel targetViewModel)
        {
            return FromDto((OrderPositionDomainEntityDto)sourceDto, useCase, targetViewModel);
        }

        IDomainEntityDto IViewModelMapper.ToDto(IUseCase useCase, IViewModel sourceViewModel)
        {
            return ToDto(useCase, sourceViewModel);
        }

        private IList<OrderPositionAdvertisementViewModel> CreateOrderPositionAdvertisementViewModel(IEnumerable<OrderPositionAdvertisementDto> dtos, ICollection<OrderPositionAdvertisementDto> selectedPositions, LookupViewModelFactory lookupFactory)
        {
            return dtos == null
                       ? new List<OrderPositionAdvertisementViewModel>()
                       : dtos.Select(dto => new OrderPositionAdvertisementViewModel(dto, selectedPositions)
                               {
                                   AdvertisementLink = lookupFactory.Create(dto.AdvertisementLink),
                                   Children = CreateOrderPositionAdvertisementViewModel(dto.Children, selectedPositions, lookupFactory),
                               })
                             .ToList();
        }

        private class LookupViewModelFactory
        {
            private readonly IUseCase _useCase;
            private readonly IViewModelIdentity _parentModelIdentity;
            private readonly ILookupFactory _lookupFactory;
            private readonly LookupPropertyFeature _propertyFeature;

            public LookupViewModelFactory(IUseCase useCase, IViewModelIdentity parentModelIdentity, ILookupFactory lookupFactory)
            {
                _useCase = useCase;
                _parentModelIdentity = parentModelIdentity;
                _lookupFactory = lookupFactory;
                _propertyFeature = new LookupPropertyFeature(EntityName.OrderPositionAdvertisement);
            }

            public LookupViewModel Create(EntityReference advertisementLink)
            {
                var model = _lookupFactory.Create(_useCase, _parentModelIdentity, _propertyFeature);
                if (advertisementLink != null)
                {
                    var entry = LookupEntry.FromReference(advertisementLink);
                    model.Items.Add(entry);
                    model.SelectedItem = entry;
                }

                return model;
            }
        }
    }
}