using System.Collections.Generic;

using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations
{
    public interface IBulkDeactivateDeniedPositionsAggregateService : IAggregateSpecificOperation<Price, DeactivateIdentity>
    {
        void Deactivate(IEnumerable<DeniedPosition> deniedPositions);
    }
}