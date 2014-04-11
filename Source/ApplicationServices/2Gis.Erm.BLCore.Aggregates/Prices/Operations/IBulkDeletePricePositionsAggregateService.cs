using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices.Operations
{
    public interface IBulkDeletePricePositionsAggregateService : IAggregateSpecificOperation<Price, DeleteIdentity>
    {
        int Delete(IEnumerable<PricePosition> pricePositions,
                   IDictionary<long, IEnumerable<AssociatedPositionsGroup>> associatedPositionsGroupsMapping,
                   IDictionary<long, IEnumerable<AssociatedPosition>> associatedPositionsMapping);
    }
}