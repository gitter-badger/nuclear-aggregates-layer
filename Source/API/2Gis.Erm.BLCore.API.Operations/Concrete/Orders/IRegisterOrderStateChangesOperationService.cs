using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderValidation;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders
{
    public interface IRegisterOrderStateChangesOperationService : IOperation<RegisterOrderStateChangesIdentity>
    {
        void Changed(IEnumerable<OrderChangesDescriptor> changedOrderDescriptors);
    }
}