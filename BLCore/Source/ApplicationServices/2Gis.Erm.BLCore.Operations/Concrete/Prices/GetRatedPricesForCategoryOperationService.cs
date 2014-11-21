using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices.Dto;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Prices
{
    public class GetRatedPricesForCategoryOperationService : IGetRatedPricesForCategoryOperationService
    {
        private readonly IPriceReadModel _priceReadModel;
        private readonly IOrderReadModel _orderReadModel;
        private readonly ICalculateCategoryRateOperationService _calculateCategoryRateOperationService;

        public GetRatedPricesForCategoryOperationService(
            IPriceReadModel priceReadModel,
            IOrderReadModel orderReadModel,
            ICalculateCategoryRateOperationService calculateCategoryRateOperationService)
        {
            _priceReadModel = priceReadModel;
            _orderReadModel = orderReadModel;
            _calculateCategoryRateOperationService = calculateCategoryRateOperationService;
        }

        public RatedPricesDto GetRatedPrices(long orderId, long pricePositionId, long[] categoryIds)
        {
            var rate = _calculateCategoryRateOperationService.GetCategoryRateForOrderCalculatedOrDefault(orderId, pricePositionId, categoryIds);
            var pricePositionCost = _priceReadModel.GetPricePositionCost(pricePositionId);
            var calculations = _orderReadModel.CalculatePricePerUnit(orderId, rate, pricePositionCost);

            return new RatedPricesDto
                {
                    PricePerUnit = calculations.PricePerUnit,
                    VatRatio = calculations.VatRatio
                };
        }
    }
}