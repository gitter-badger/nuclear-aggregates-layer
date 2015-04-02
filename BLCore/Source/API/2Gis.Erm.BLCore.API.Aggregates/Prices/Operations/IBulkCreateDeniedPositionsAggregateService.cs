using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations
{
    public interface IBulkCreateDeniedPositionsAggregateService : IAggregateSpecificOperation<Price, CreateIdentity>
    {
        void Create(IEnumerable<DeniedPosition> deniedPositions);
    }
}