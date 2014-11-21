﻿using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Release;

namespace DoubleGis.Erm.BLCore.API.Releasing.Releases
{
    public interface IStartSimplifiedReleaseOperationService : IOperation<StartSimplifiedReleaseIdentity>
    {
        ReleaseStartingResult Start(long organizationUnitId, TimePeriod period, bool isBeta);
    }
}
