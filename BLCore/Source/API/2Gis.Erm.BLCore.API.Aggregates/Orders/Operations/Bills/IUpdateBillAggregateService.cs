using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Bills
{
    public interface IUpdateBillAggregateService : IAggregateSpecificOperation<Order, UpdateIdentity>
    {
        void Update(Bill bill, IEnumerable<Bill> bills, Order order);
    }
}
