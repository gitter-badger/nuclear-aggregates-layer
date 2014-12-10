using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders.Bills
{
    public interface IDeleteOrderBillsOperationService : IEntityOperation<Bill>, IOperation<DeleteIdentity>
    {
        void Delete(long orderId);
    }
}