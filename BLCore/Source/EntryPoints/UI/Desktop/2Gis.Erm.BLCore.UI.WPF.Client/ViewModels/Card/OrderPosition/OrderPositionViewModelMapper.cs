using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card.OrderPosition.DTOs;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Lookup;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Mappers;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card.OrderPosition
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
            
            // FIXME {a.rechkalov, 2013-06-28}: здесь пока кладём на пришедшую от сервера модель, а это надо переделать
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
                _propertyFeature = new LookupPropertyFeature(EntityType.Instance.OrderPositionAdvertisement());
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