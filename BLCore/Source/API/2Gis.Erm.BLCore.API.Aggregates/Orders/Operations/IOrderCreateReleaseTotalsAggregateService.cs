using System.Collections.Generic;

using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations
{
    public interface IOrderCreateReleaseTotalsAggregateService : IAggregateSpecificOperation<Order, CreateIdentity>
    {
        void Create(IEnumerable<OrderReleaseTotal> releaseTotals);
    }
}