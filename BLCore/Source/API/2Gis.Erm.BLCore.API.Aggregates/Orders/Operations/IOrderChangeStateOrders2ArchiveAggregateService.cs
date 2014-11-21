using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations
{
    public interface IOrderChangeStateOrders2ArchiveAggregateService : IAggregatePartRepository<Order>
    {
        IEnumerable<ChangesDescriptor> Archiving(IEnumerable<Order> orders);
    }
}