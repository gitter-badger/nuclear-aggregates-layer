using DoubleGis.Erm.BLCore.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.Prices.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices.Dto;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Prices
{
    // TODO {a.tukaev, 25.03.2014}: Лучше чтобы в назание абстракций и реализаций для OperationsService оканчивались именно на OperationService, т.к. сервисов разного вида у нас много 
    public class GetRatedPricesForCategoryOperationService : IGetRatedPricesForCategoryOperationService
    {
        private readonly IPriceReadModel _priceReadModel;
        private readonly IOrderReadModel _orderReadModel;
        private readonly IFirmReadModel _firmReadModel;
        private readonly ICalculateCategoryRateOperationService _calculateCategoryRateOperationService;

        public GetRatedPricesForCategoryOperationService(IPriceReadModel priceReadModel,
                                                IOrderReadModel orderReadModel,
                                                ICalculateCategoryRateOperationService calculateCategoryRateOperationService, IFirmReadModel firmReadModel)
        {
            _priceReadModel = priceReadModel;
            _orderReadModel = orderReadModel;
            _calculateCategoryRateOperationService = calculateCategoryRateOperationService;
            _firmReadModel = firmReadModel;
        }

        public RatedPricesDto GetRatedPrices(long orderId, long pricePositionId, long? categoryId)
        {
            var firmId = _firmReadModel.GetOrderFirmId(orderId);
            var rate = _calculateCategoryRateOperationService.CalculateCategoryRate(firmId, pricePositionId, categoryId, false);

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