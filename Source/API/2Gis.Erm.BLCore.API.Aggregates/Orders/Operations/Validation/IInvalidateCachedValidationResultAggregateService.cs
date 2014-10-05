using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Validation
{
    public interface IInvalidateCachedValidationResultAggregateService : ISimplifiedModelConsumer
    {
        void Invalidate(IEnumerable<OrderChangesDescriptor> invalidatedOrderDescriptors);
    }
}