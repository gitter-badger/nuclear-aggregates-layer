using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations
{
    public interface IActivatePriceAggregateService : IAggregateSpecificService<Price, ActivateIdentity>
    {
        int Activate(Price price);
    }
}