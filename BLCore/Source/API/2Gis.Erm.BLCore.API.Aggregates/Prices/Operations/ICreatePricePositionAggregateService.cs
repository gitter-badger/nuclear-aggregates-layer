using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations
{
    public interface ICreatePricePositionAggregateService : IAggregateSpecificOperation<Price, CreateIdentity>
    {
        void Create(PricePosition pricePosition);
    }
}