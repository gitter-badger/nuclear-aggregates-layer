using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.CostCalculation
{
    public interface ICalculateOrderCostService : IOperation<CalculateOrderCostIdentity>
    {
        CalculationResult CalculateOrderProlongation(long orderId);
    }
}
