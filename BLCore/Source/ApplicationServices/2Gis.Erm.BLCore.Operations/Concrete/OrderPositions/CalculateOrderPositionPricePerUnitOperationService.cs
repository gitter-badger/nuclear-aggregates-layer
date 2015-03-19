using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions.Dto;
using DoubleGis.Erm.BLCore.API.Operations.Special.CostCalculation;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.OrderPositions
{
    public sealed class CalculateOrderPositionPricePerUnitOperationService : ICalculateOrderPositionPricePerUnitOperationService
    {
        private readonly IOrderReadModel _orderReadModel;
        private readonly ICostCalculator _costCalculator;

        public CalculateOrderPositionPricePerUnitOperationService(IOrderReadModel orderReadModel, ICostCalculator costCalculator)
        {
            _orderReadModel = orderReadModel;
            _costCalculator = costCalculator;
        }

        public OrderPositionPricePerUnitDto CalculatePricePerUnit(long orderId, decimal categoryRate, decimal pricePositionCost)
        {
            var orderType = _orderReadModel.GetOrderType(orderId);

            if (orderType == OrderType.SelfAds || orderType == OrderType.SocialAds)
            {
                return new OrderPositionPricePerUnitDto { PricePerUnit = 0m, PricePerUnitWithVat = 0m };
            }

            bool showVat;
            var vatRate = _orderReadModel.GetVatRate(orderId, out showVat);
            var calcResult = _costCalculator.CalculatePricePerUnit(pricePositionCost, categoryRate, vatRate, showVat);

            return new OrderPositionPricePerUnitDto
                       {
                           PricePerUnit = calcResult.PricePerUnit,
                           PricePerUnitWithVat = calcResult.PricePerUnitWithVat
                       };
        }
    }
}
