using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderPosition;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions
{
    public interface ICalculateCategoryRateOperationService : IOperation<CalculateCategoryRateIdentity>
    {
        decimal CalculateCategoryRate(long firmId, long pricePositionId, long? categoryId, bool strictMode);
        decimal CalculateCategoryRate(long firmId, long pricePositionId, bool strictMode);
    }
}