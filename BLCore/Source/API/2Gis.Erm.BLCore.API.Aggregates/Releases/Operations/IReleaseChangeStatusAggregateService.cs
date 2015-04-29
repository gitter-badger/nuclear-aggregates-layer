using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Releases.Operations
{
    public interface IReleaseChangeStatusAggregateService : IAggregatePartRepository<ReleaseInfo>
    {
        void InProgressInternalProcessingStarted(ReleaseInfo release, string changesDescription);
        void InProgressWaitingExternalProcessing(ReleaseInfo release);
        void Finished(ReleaseInfo release, ReleaseStatus targetStatus, string changesDescription);
        void Reverting(ReleaseInfo release, string changesDescription);
        void Reverted(ReleaseInfo release);
        void SetPreviousStatus(ReleaseInfo release, ReleaseStatus previousStatus, string changesDescription);
    }
}