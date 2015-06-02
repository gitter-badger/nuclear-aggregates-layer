using DoubleGis.Erm.Platform.API.Core.Operations;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Reopen
{
    public interface IReopenOperationService : IOperation<ReopenIdentity>
    {
        void Reopen(long entityId);
    }    
}
