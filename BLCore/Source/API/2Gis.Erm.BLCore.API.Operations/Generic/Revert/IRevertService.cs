using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Revert;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Revert
{
    public interface IRevertService : IOperation<RevertIdentity>
    {
        void Revert(long entityId);
    }    
}
