using System;

using DoubleGis.Erm.BLCore.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.OrderPositions
{
    public sealed class CalculateOrderPositionPricesHandler : RequestHandler<CalculateOrderPositionPricesRequest, CalculateOrderPositionPricesResponse>
    {
        private readonly IOrderRepository _orderRepository;

        public CalculateOrderPositionPricesHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        protected override CalculateOrderPositionPricesResponse Handle(CalculateOrderPositionPricesRequest request)
        {
            var order = _orderRepository.GetOrder(request.OrderId);
            var priceCalculations = _orderRepository.CalculatePricePerUnit(request.OrderId, request.CategoryRate, request.Cost);

            var pricePerUnit = Math.Round(priceCalculations.PricePerUnit, 2, MidpointRounding.ToEven);
            var pricePerUnitWithVat = pricePerUnit * (decimal.One + priceCalculations.VatRatio);
            pricePerUnitWithVat = Math.Round(pricePerUnitWithVat, 2, MidpointRounding.ToEven);

            var recalculatedResult = _orderRepository.Recalculate(
                request.Amount,
                priceCalculations.PricePerUnit,
                pricePerUnitWithVat,
                order.ReleaseCountFact,
                request.CalculateDiscountViaPercent,
                request.DiscountPercent,
                request.DiscountSum);

            var response = new CalculateOrderPositionPricesResponse
                {
                    PricePerUnit = pricePerUnit,
                    PricePerUnitWithVat = pricePerUnitWithVat,
                    DiscountPercent = recalculatedResult.DiscountPercent,
                    DiscountSum = recalculatedResult.DiscountSum,
                    PayablePlan = recalculatedResult.PayablePlan,
                    PayablePlanWoVat = recalculatedResult.PayablePlanWoVat,
                    PayablePrice = recalculatedResult.PayablePrice,
                    ShipmentPlan = recalculatedResult.ShipmentPlan,
                    CategoryRate = priceCalculations.CategoryRate,
                };

            return response;
        }
    }
}