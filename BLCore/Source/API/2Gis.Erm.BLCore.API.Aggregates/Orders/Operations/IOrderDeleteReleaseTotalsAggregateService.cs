using System.Collections.Generic;

using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations
{
    public interface IOrderDeleteReleaseTotalsAggregateService : IAggregateSpecificService<Order, DeleteIdentity>
    {
        void Delete(IEnumerable<OrderReleaseTotal> releaseTotals);
    }
}