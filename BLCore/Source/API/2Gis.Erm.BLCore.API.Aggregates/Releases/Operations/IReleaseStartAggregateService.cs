using DoubleGis.Erm.Platform.API.Core;
using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Releases.Operations
{
    public interface IReleaseStartAggregateService : IAggregateSpecificOperation<ReleaseInfo, CreateIdentity>
    {
        ReleaseInfo Start(int countryCode, long organizationUnitId, TimePeriod period, bool isBeta, ReleaseStatus targetStatus);
    }
}