using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Cancel;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Cancel
{
    public interface ICancelOperationService : IOperation<CancelIdentity>
    {
        void Cancel(long entityId);
    }
}
