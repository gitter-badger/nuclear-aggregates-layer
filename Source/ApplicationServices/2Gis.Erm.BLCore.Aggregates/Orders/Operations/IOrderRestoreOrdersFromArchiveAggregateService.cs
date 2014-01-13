using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.Operations
{
    public interface IOrderRestoreOrdersFromArchiveAggregateService : IAggregatePartRepository<Order>
    {
        IEnumerable<ChangesDescriptor> Restore(IEnumerable<Order> orders);
    }
}