using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderPosition;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions
{
    public interface ICalculateCategoryRateOperationService : IOperation<CalculateCategoryRateIdentity>
    {
        decimal GetCategoryRateForOrderCalculated(long orderId, long pricePositionId, long[] categoryIds);
        decimal GetCategoryRateForFirmCalculated(long? firmId, long pricePositionId, long[] categoryIds);
        decimal GetCategoryRateForOrderCalculatedOrDefault(long orderId, long pricePositionId, long[] categoryIds);
    }
}