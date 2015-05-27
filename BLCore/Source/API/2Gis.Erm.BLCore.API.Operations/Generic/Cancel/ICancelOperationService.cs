using DoubleGis.Erm.Platform.API.Core.Operations;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Cancel
{
    public interface ICancelOperationService : IOperation<CancelIdentity>
    {
        void Cancel(long entityId);
    }
}
