using System.Collections.Generic;

using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations
{
    public interface IBulkDeletePricePositionsAggregateService : IAggregateSpecificOperation<Price, DeleteIdentity>
    {
        int Delete(IEnumerable<PricePosition> pricePositions,
                   IDictionary<long, IEnumerable<AssociatedPositionsGroup>> associatedPositionsGroupsMapping,
                   IDictionary<long, IEnumerable<AssociatedPosition>> associatedPositionsMapping);
    }
}