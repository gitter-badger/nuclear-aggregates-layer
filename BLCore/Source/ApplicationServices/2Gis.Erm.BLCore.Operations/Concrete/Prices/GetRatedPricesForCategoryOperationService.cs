using DoubleGis.Erm.BLCore.API.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices.Dto;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Prices
{
    public class GetRatedPricesForCategoryOperationService : IGetRatedPricesForCategoryOperationService
    {
        private readonly IPriceReadModel _priceReadModel;
        private readonly ICalculateCategoryRateOperationService _calculateCategoryRateOperationService;
        private readonly ICalculateOrderPositionPricePerUnitOperationService _calculateOrderPositionPricePerUnitOperationService;

        public GetRatedPricesForCategoryOperationService(
            IPriceReadModel priceReadModel,
            ICalculateCategoryRateOperationService calculateCategoryRateOperationService,
            ICalculateOrderPositionPricePerUnitOperationService calculateOrderPositionPricePerUnitOperationService)
        {
            _priceReadModel = priceReadModel;
            _calculateCategoryRateOperationService = calculateCategoryRateOperationService;
            _calculateOrderPositionPricePerUnitOperationService = calculateOrderPositionPricePerUnitOperationService;
        }

        public RatedPricesDto GetRatedPrices(long orderId, long pricePositionId, long[] categoryIds)
        {
            var rate = _calculateCategoryRateOperationService.GetCategoryRateForOrderCalculatedOrDefault(orderId, pricePositionId, categoryIds);
            var pricePositionCost = _priceReadModel.GetPricePositionCost(pricePositionId);
            var calculations = _calculateOrderPositionPricePerUnitOperationService.CalculatePricePerUnit(orderId, rate, pricePositionCost);

            return new RatedPricesDto
                {
                    PricePerUnit = calculations.PricePerUnit,
                    PricePerUnitWithVat = calculations.PricePerUnitWithVat
                };
        }
    }
}