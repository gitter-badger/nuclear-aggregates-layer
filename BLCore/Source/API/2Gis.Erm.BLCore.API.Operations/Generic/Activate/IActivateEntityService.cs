using DoubleGis.Erm.Platform.API.Core.Operations;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Activate
{
    public interface IActivateEntityService : IOperation<ActivateIdentity>
    {
        int Activate(long entityId);
    }
}
