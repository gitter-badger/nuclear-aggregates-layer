using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations
{
    public interface IOrderRestoreOrdersFromArchiveAggregateService : IAggregatePartService<Order>
    {
        IEnumerable<ChangesDescriptor> Restore(IEnumerable<Order> orders);
    }
}