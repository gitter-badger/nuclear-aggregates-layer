using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Activate
{
    public interface IActivateEntityService : IOperation<ActivateIdentity>
    {
        int Activate(long entityId);
    }
}
