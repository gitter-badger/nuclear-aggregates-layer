using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Validation
{
    public interface IAttachValidResultToCacheAggregateService : ISimplifiedModelConsumer
    {
        void Attach(IEnumerable<OrderValidationCacheEntry> validResults);
    }
}