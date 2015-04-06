using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders.Bills
{
    public interface IDeleteOrderBillsOperationService : IOperation<DeleteOrderBillsIdentity>
    {
        void Delete(long orderId);
    }
}