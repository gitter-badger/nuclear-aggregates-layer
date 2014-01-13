using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Releases.Operations
{
    public interface IReleaseStartAggregateService : IAggregateSpecificOperation<ReleaseInfo, CreateIdentity>
    {
        ReleaseInfo Start(long organizationUnitId, TimePeriod period, bool isBeta, ReleaseStatus targetStatus);
    }
}