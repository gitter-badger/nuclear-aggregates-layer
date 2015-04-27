using DoubleGis.Erm.BLCore.Aggregates.Positions;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions.Dto;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.OrderPositions
{
    public class ViewOrderPositionOperationService : IViewOrderPositionOperationService
    {
        private readonly IOrderReadModel _orderReadModel;
        private readonly IPriceReadModel _priceReadModel;
        private readonly ICalculateCategoryRateOperationService _calculateCategoryRateOperationService;
        private readonly IGetAvailableBindingObjectsOperationService _formAvailableBindingObjectsOperationService;
        private readonly ICalculateOrderPositionPricePerUnitOperationService _calculateOrderPositionPricePerUnitOperationService;

        public ViewOrderPositionOperationService(IOrderReadModel orderReadModel,
                                                 IPriceReadModel priceReadModel,
                                                 ICalculateCategoryRateOperationService calculateCategoryRateOperationService,
                                                 IGetAvailableBindingObjectsOperationService formAvailableBindingObjectsOperationService,
                                                 ICalculateOrderPositionPricePerUnitOperationService calculateOrderPositionPricePerUnitOperationService)
        {
            _orderReadModel = orderReadModel;
            _priceReadModel = priceReadModel;
            _calculateCategoryRateOperationService = calculateCategoryRateOperationService;
            _formAvailableBindingObjectsOperationService = formAvailableBindingObjectsOperationService;
            _calculateOrderPositionPricePerUnitOperationService = calculateOrderPositionPricePerUnitOperationService;
        }

        public OrderPositionWithSchemaDto ViewOrderPosition(long orderId, long pricePositionId, long? orderPositionId, bool includeHidden)
        {
            var order = _orderReadModel.GetOrderLinkingObjectsDto(orderId);
            var positionInfo = _priceReadModel.GetPricePositionDetailedInfo(pricePositionId);
            var categoryRate = _calculateCategoryRateOperationService.GetCategoryRateForOrderCalculatedOrDefault(orderId, pricePositionId, null);
            var priceCalulations = _calculateOrderPositionPricePerUnitOperationService.CalculatePricePerUnit(orderId, categoryRate, positionInfo.PricePositionCost);            
            var linkingObjectsSchema = _formAvailableBindingObjectsOperationService.GetLinkingObjectsSchema(orderId, pricePositionId, includeHidden, orderPositionId);

            return new OrderPositionWithSchemaDto
            {
                OrderReleaseCountFact = order.ReleaseCountFact,
                OrderReleaseCountPlan = order.ReleaseCountPlan,

                PlatformName = positionInfo.Platform,
                PricePositionAmount = positionInfo.Amount,
                AmountSpecificationMode = positionInfo.AmountSpecificationMode,
                PricePositionCost = positionInfo.PricePositionCost,
                IsPositionComposite = positionInfo.Position.IsComposite,
                IsPositionCategoryBound = positionInfo.RateType == PricePositionRateType.BoundCategory,
                LinkingObjectType = positionInfo.Position.BindingObjectTypeEnum,

                PricePerUnit = priceCalulations.PricePerUnit,
                PricePerUnitWithVat = priceCalulations.PricePerUnitWithVat,

                LinkingObjectsSchema = linkingObjectsSchema,
                KeepCategoriesSynced = positionInfo.Position.KeepCategoriesSynced(),
                SalesModel = (int)positionInfo.Position.SalesModel
            };
        }
    }
}