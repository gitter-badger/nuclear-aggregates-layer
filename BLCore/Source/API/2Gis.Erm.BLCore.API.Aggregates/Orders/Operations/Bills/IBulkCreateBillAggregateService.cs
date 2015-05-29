using System.Collections.Generic;

using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Bills
{
    public interface IBulkCreateBillAggregateService : IAggregateSpecificService<Order, BulkCreateIdentity>
    {
        void Create(Order order, IEnumerable<Bill> bills);
    }
}
