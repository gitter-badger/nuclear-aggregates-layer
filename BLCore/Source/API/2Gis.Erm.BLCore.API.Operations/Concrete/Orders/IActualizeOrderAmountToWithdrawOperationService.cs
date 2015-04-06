using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders
{
    public interface IActualizeOrderAmountToWithdrawOperationService : IOperation<ActualizeOrderAmountToWithdrawIdentity>
    {
        void Actualize(long orderId);
    }
}