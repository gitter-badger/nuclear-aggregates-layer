using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations
{
    public interface IBulkActivateDeniedPositionsAggregateService : IAggregateSpecificOperation<Price, ActivateIdentity>
    {
        int Activate(IEnumerable<DeniedPosition> deniedPositions);
    }
}