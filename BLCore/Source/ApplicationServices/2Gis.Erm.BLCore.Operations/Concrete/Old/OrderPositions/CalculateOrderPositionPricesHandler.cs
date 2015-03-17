using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.OrderPositions
{
    public sealed class CalculateOrderPositionPricesHandler : RequestHandler<CalculateOrderPositionPricesRequest, CalculateOrderPositionPricesResponse>
    {
        private readonly IOrderReadModel _orderReadModel;

        public CalculateOrderPositionPricesHandler(IOrderReadModel orderReadModel)
        {
            _orderReadModel = orderReadModel;
        }

        protected override CalculateOrderPositionPricesResponse Handle(CalculateOrderPositionPricesRequest request)
        {
            var order = _orderReadModel.GetOrderSecure(request.OrderId);
            if (order == null)
            {
                throw new EntityNotFoundException(typeof(Order), request.OrderId);
            }

            var priceCalculations = _orderReadModel.CalculatePricePerUnit(request.OrderId, request.CategoryRate, request.Cost);

            var pricePerUnit = Math.Round(priceCalculations.PricePerUnit, 2, MidpointRounding.ToEven);
            var pricePerUnitWithVat = pricePerUnit * (decimal.One + priceCalculations.VatRatio);
            pricePerUnitWithVat = Math.Round(pricePerUnitWithVat, 2, MidpointRounding.ToEven);

            var recalculatedResult = _orderReadModel.Recalculate(
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