using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderValidation;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders
{
    public interface IRegisterOrderStateChangesOperationService : IOperation<RegisterOrderStateChangesIdentity>
    {
        // FIXME {i.maslennikov, 08.10.2014}: Давай оставим какую-то одну сигнатуру, видимо более широкую. Может быть и этот OrderChangesDescriptor не использовать в сигнатуре и создавать внутри?
        void Changed(IEnumerable<OrderChangesDescriptor> changedOrderDescriptors);
        void Changed(long orderId, params OrderValidationRuleGroup[] aspects);
    }
}