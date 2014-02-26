using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.OrderPositions
{
    // FIXME {all, 11.02.2014}: Соответствующие реквесты никто не создает, хендлер надо выпилить?
    public sealed class CalculatePositionPricesWithDiscountDistributionHandler :
        RequestHandler<CalculatePositionPricesWithDiscountDistributionRequest, CalculatePositionPricesWithDiscountDistributionResponse>
    {
        private readonly IPublicService _publicService;

        public CalculatePositionPricesWithDiscountDistributionHandler(IPublicService publicService)
        {
            _publicService = publicService;
        }

        protected override CalculatePositionPricesWithDiscountDistributionResponse Handle(CalculatePositionPricesWithDiscountDistributionRequest request)
        {
            var discountSum = 0M;
            var result = new CalculatePositionPricesWithDiscountDistributionResponse
                {
                    Calculations = new CalculateOrderPositionPricesResponse[request.Costs.Length]
                };

            for (var index = 0; index < request.Costs.Length - 1; index++)
            {
                result.Calculations[index] =
                    (CalculateOrderPositionPricesResponse)_publicService.Handle(new CalculateOrderPositionPricesRequest
                        {
                            Cost = request.Costs[index],
                            DiscountPercent = request.DiscountPercent,
                            DiscountSum = request.DiscountSum,

                            CalculateDiscountViaPercent = true,
                            OrderId = request.OrderId,
                            CategoryRate = request.CategoryRates[index],
                            Amount = request.Amounts[index]
                        });

                discountSum += result.Calculations[index].DiscountSum;
            }

            // последнюю позицию надо рассчитать не в процентах, чтобы сохранить точность
            result.Calculations[request.Costs.Length - 1] =
                (CalculateOrderPositionPricesResponse)_publicService.Handle(new CalculateOrderPositionPricesRequest
                    {
                        Cost = request.Costs[request.Costs.Length - 1],
                        DiscountPercent = request.DiscountPercent,
                        DiscountSum = request.DiscountSum - discountSum,

                        CalculateDiscountViaPercent = false,
                        OrderId = request.OrderId,
                        CategoryRate = request.CategoryRates[request.Costs.Length - 1],
                        Amount = request.Amounts[request.Costs.Length - 1]
                    });

            return result;
        }
    }
}
