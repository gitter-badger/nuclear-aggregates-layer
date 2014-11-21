using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Release;

namespace DoubleGis.Erm.BLCore.API.Releasing.Releases
{
    public interface IRevertReleaseOperationService : IOperation<RevertReleaseIdentity>
    {
        ReleaseRevertingResult Revert(long organizationUnitId, TimePeriod period, string comment);
    }
}
