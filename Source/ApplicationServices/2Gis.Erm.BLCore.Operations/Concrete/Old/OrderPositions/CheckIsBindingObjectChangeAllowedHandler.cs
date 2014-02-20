using System;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.OrderPositions
{
    public sealed class CheckIsBindingObjectChangeAllowedHandler : RequestHandler<CheckIsBindingObjectChangeAllowedRequest, CheckIsBindingObjectChangeAllowedResponse>
    {
        // элементы привязки, которые можно менять.
        // согласно http://confluence.dvlp.2gis.local/pages/viewpage.action?pageId=95750732
        private static readonly PositionBindingObjectType[] AllowedPositionTypes = new[]
            {
                PositionBindingObjectType.AddressSingle,
                PositionBindingObjectType.AddressMultiple,
                PositionBindingObjectType.CategorySingle,
                PositionBindingObjectType.CategoryMultiple,
                PositionBindingObjectType.CategoryMultipleAsterix,
                PositionBindingObjectType.AddressCategorySingle,
                PositionBindingObjectType.AddressCategoryMultiple,
                PositionBindingObjectType.AddressFirstLevelCategorySingle,
                PositionBindingObjectType.AddressFirstLevelCategoryMultiple,
            };

        // Состояния заказа, в котором можно менять объекты привязки
        private static readonly OrderState[] AllowedOrderStates =
            {
                OrderState.Approved,
            };

        private readonly IOrderReadModel _orderReadModel;

        public CheckIsBindingObjectChangeAllowedHandler(IOrderReadModel orderReadModel)
        {
            _orderReadModel = orderReadModel;
        }

        protected override CheckIsBindingObjectChangeAllowedResponse Handle(CheckIsBindingObjectChangeAllowedRequest request)
        {
            var info = _orderReadModel.GetOrderPositionInfo(request.OrderPositionId);
            
            if(info == null)
                return new CheckIsBindingObjectChangeAllowedResponse(BLResources.EntityNotFound);

            if (!request.SkipAdvertisementCountCheck && info.AdverisementCount != request.AdvertisementCount)
                return new CheckIsBindingObjectChangeAllowedResponse(BLResources.InvalidBindingObjectCount,
                                                                info.AdverisementCount, request.AdvertisementCount) { OrderId = info.OrderId };

            if (!AllowedPositionTypes.Contains(info.BindingType))
                return new CheckIsBindingObjectChangeAllowedResponse(BLResources.InvalidOrderPositionBindingType,
                                                                info.BindingType,
                                                                String.Join(", ", AllowedPositionTypes)) { OrderId = info.OrderId };

            if (!AllowedOrderStates.Contains(info.OrderWorkflowSate))
                return new CheckIsBindingObjectChangeAllowedResponse(BLResources.InvalidOrderWorkflowSate,
                                                                info.OrderWorkflowSate,
                                                                String.Join(", ", AllowedOrderStates)) { OrderId = info.OrderId };

            return new CheckIsBindingObjectChangeAllowedResponse() { OrderId = info.OrderId };
        }
    }
}