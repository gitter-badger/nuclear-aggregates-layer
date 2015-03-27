using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations
{
    public interface IUpdatePriceAggregateService : IAggregateSpecificOperation<Price, UpdateIdentity>
    {
        int Update(Price price);
    }
}