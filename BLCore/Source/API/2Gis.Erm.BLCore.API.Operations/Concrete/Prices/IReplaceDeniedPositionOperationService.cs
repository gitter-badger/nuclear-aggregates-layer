using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.DeniedPosition;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices
{
    public interface IReplaceDeniedPositionOperationService : IOperation<ReplaceDeniedPositionIdentity>
    {
        void Replace(long deniedPositionId, long newPositionDeniedId);
    }
}
