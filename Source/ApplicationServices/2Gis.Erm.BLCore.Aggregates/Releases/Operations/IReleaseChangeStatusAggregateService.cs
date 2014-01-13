using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Releases.Operations
{
    public interface IReleaseChangeStatusAggregateService : IAggregatePartRepository<ReleaseInfo>
    {
        void ChangeStatus(ReleaseInfo release, ReleaseStatus targetStatus, string changesDescription);
        void Finished(ReleaseInfo release,  ReleaseStatus targetStatus, string changesDescription);
    }
}
