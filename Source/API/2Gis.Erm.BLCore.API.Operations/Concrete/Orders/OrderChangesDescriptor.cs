using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.OrderValidation;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders
{
    public sealed class OrderChangesDescriptor
    {
        public long OrderId { get; set; }
        public IEnumerable<OrderValidationRuleGroup> ChangedAspects { get; set; }
    }
}