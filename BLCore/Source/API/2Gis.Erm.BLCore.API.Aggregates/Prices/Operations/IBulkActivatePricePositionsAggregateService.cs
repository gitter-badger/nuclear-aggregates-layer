using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations
{
    public interface IBulkActivatePricePositionsAggregateService : IAggregateSpecificOperation<Price, ActivateIdentity>
    {
        int Activate(IEnumerable<PricePosition> pricePositions,
                     IDictionary<long, IEnumerable<AssociatedPositionsGroup>> associatedPositionsGroupsMapping,
                     IDictionary<long, IEnumerable<AssociatedPosition>> associatedPositionsMapping);
    }
}