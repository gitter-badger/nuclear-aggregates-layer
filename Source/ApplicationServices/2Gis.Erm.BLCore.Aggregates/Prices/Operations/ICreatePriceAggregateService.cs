using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices.Operations
{
    public interface ICreatePriceAggregateService : IAggregateSpecificOperation<Price, CreateIdentity>
    {
        int Create(Price price, long currencyId);
    }
}