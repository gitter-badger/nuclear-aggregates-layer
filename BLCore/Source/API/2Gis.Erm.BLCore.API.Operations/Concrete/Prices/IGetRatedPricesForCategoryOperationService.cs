using DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices.Dto;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Price;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices
{
    public interface IGetRatedPricesForCategoryOperationService : IOperation<GetRatedPricesForCategoryIdentity>
    {
        RatedPricesDto GetRatedPrices(long orderId, long pricePositionId, long[] categoryIds);
    }
}