using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions.Dto;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderPosition;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions
{
    public interface ICalculateOrderPositionPricePerUnitOperationService : IOperation<CalculateOrderPositionPricePerUnitIdentity>
    {
        OrderPositionPricePerUnitDto CalculatePricePerUnit(long orderId, decimal categoryRate, decimal pricePositionCost);
    }
}