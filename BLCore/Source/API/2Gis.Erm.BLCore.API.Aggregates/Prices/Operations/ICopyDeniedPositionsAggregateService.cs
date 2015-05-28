using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Dto;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.DeniedPosition;

using NuClear.Aggregates;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations
{
    public interface ICopyDeniedPositionsAggregateService : IAggregateSpecificOperation<Price, CopyDeniedPositionsIdentity>
    {
        void Copy(IEnumerable<DeniedPositionToCopyDto> deniedPositionsToCopy, long targetPriceId);
    }
}