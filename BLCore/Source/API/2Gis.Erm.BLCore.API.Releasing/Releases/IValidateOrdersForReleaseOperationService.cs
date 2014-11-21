using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Release;

namespace DoubleGis.Erm.BLCore.API.Releasing.Releases
{
    public interface IValidateOrdersForReleaseOperationService : IOperation<ValidateOrdersForReleaseIdentity>
    {
        IEnumerable<ReleaseProcessingMessage> Validate(long organizationUnitId, TimePeriod period, bool isBeta);
    }
}