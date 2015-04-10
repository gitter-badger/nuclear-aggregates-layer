using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.OrderPositions
{
    public sealed class CalculateOrderPositionPricesHandler : RequestHandler<CalculateOrderPositionPricesRequest, CalculateOrderPositionPricesResponse>
    {
        private readonly IOrderReadModel _orderReadModel;
        private readonly ICalculateOrderPositionPricePerUnitOperationService _calculateOrderPositionPricePerUnitOperationService;

        public CalculateOrderPositionPricesHandler(IOrderReadModel orderReadModel, ICalculateOrderPositionPricePerUnitOperationService calculateOrderPositionPricePerUnitOperationService)
        {
            _orderReadModel = orderReadModel;
            _calculateOrderPositionPricePerUnitOperationService = calculateOrderPositionPricePerUnitOperationService;
        }

        protected override CalculateOrderPositionPricesResponse Handle(CalculateOrderPositionPricesRequest request)
        {
            var order = _orderReadModel.GetOrderSecure(request.OrderId);
            if (order == null)
            {
                throw new EntityNotFoundException(typeof(Order), request.OrderId);
            }

            var priceCalculations = _calculateOrderPositionPricePerUnitOperationService.CalculatePricePerUnit(request.OrderId, request.CategoryRate, request.Cost);

            var recalculatedResult = _orderReadModel.Recalculate(request.Amount,
                                                                 priceCalculations.PricePerUnit,
                                                                 priceCalculations.PricePerUnitWithVat,
                                                                 order.ReleaseCountFact,
                                                                 request.CalculateDiscountViaPercent,
                                                                 request.DiscountPercent,
                                                                 request.DiscountSum);

            var response = new CalculateOrderPositionPricesResponse
                {
                    PricePerUnit = priceCalculations.PricePerUnit,
                    PricePerUnitWithVat = priceCalculations.PricePerUnitWithVat,
                    DiscountPercent = recalculatedResult.DiscountPercent,
                    DiscountSum = recalculatedResult.DiscountSum,
                    PayablePlan = recalculatedResult.PayablePlan,
                    PayablePlanWoVat = recalculatedResult.PayablePlanWoVat,
                    PayablePrice = recalculatedResult.PayablePrice,
                    ShipmentPlan = recalculatedResult.ShipmentPlan,
                };

            return response;
        }
    }
}