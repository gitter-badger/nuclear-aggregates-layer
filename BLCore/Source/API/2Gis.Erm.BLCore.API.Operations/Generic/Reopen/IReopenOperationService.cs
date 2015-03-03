using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Reopen;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Reopen
{
    public interface IReopenOperationService : IOperation<ReopenIdentity>
    {
        void Reopen(long entityId);
    }    
}
