using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Release;

namespace DoubleGis.Erm.BLCore.API.Releasing.Releases
{
    public interface IStartReleaseOperationService : IOperation<StartReleaseIdentity>
    {
        ReleaseStartingResult Start(int organizationUnitDgppId, TimePeriod period, bool isBeta, bool canIgnoreBlockingErrors);
    }
}
