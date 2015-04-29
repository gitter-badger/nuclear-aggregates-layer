using System.Collections.Generic;

using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations
{
    public interface IBulkCreateAssociatedPositionsGroupsAggregateService : IAggregateSpecificOperation<Price, CreateIdentity>
    {
        int Create(IEnumerable<AssociatedPositionsGroup> associatedPositionsGroups, long pricePositionId);
    }
}