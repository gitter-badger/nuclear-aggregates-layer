﻿using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations
{
    public interface IBulkDeactivatePricePositionsAggregateService : IAggregateSpecificOperation<Price, DeactivateIdentity>
    {
        int Deactivate(IEnumerable<PricePosition> pricePositions,
                       IDictionary<long, IEnumerable<AssociatedPositionsGroup>> associatedPositionsGroupsMapping,
                       IDictionary<long, IEnumerable<AssociatedPosition>> associatedPositionsMapping);
    }
}