using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.CancelActivity;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.CancelActivity
{
    public interface ICancelActivityService : IOperation<CancelActivityIdentity>
    {
        CancelActivityResult Cancel(long entityId);
    }
}
