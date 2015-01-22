using DoubleGis.Erm.Platform.API.Core.Operations;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Append
{
    public interface IAppendEntityService : IOperation<AppendIdentity>
    {
        void Append(AppendParams appendParams);
    }
}
