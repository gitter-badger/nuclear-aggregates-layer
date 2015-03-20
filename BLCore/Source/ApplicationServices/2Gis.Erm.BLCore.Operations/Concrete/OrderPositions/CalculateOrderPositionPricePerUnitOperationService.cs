using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions.Dto;
using DoubleGis.Erm.BLCore.API.Operations.Special.CostCalculation;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderPosition;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.OrderPositions
{
    public sealed class CalculateOrderPositionPricePerUnitOperationService : ICalculateOrderPositionPricePerUnitOperationService
    {
        private readonly IOrderReadModel _orderReadModel;
        private readonly ICostCalculator _costCalculator;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public CalculateOrderPositionPricePerUnitOperationService(IOrderReadModel orderReadModel, ICostCalculator costCalculator, IOperationScopeFactory operationScopeFactory)
        {
            _orderReadModel = orderReadModel;
            _costCalculator = costCalculator;
            _operationScopeFactory = operationScopeFactory;
        }

        public OrderPositionPricePerUnitDto CalculatePricePerUnit(long orderId, decimal categoryRate, decimal pricePositionCost)
        {
            using (var scope = _operationScopeFactory.CreateNonCoupled<CalculateOrderPositionPricePerUnitIdentity>())
            {
                var orderType = _orderReadModel.GetOrderType(orderId);

                if (orderType == OrderType.SelfAds || orderType == OrderType.SocialAds)
                {
                    scope.Complete();
                    return new OrderPositionPricePerUnitDto { PricePerUnit = 0m, PricePerUnitWithVat = 0m };
                }

                bool showVat;
                var vatRate = _orderReadModel.GetVatRate(orderId, out showVat);
                var calcResult = _costCalculator.CalculatePricePerUnit(pricePositionCost, categoryRate, vatRate, showVat);

                scope.Complete();
                return new OrderPositionPricePerUnitDto
                {
                    PricePerUnit = calcResult.PricePerUnit,
                    PricePerUnitWithVat = calcResult.PricePerUnitWithVat
                };
            }
        }
    }
}
