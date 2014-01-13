using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Append
{
    public interface IAppendEntityService : IOperation<AppendIdentity>
    {
        void Append(AppendParams appendParams);
    }
}
