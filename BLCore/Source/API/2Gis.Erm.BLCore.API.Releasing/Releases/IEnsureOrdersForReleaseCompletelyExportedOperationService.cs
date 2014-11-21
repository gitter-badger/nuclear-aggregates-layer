using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Release;

namespace DoubleGis.Erm.BLCore.API.Releasing.Releases
{
    public interface IEnsureOrdersForReleaseCompletelyExportedOperationService : IOperation<EnsureOrdersForReleaseCompletelyExportedIdentity>
    {
        bool IsExported(
            long releaseId, 
            long organizationUnitId,
            int organizationUnitDgppId,
            TimePeriod period, 
            bool isBeta);
    }
}