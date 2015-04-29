using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Deals.Operations
{
    public interface IUpdateLegalPersonDealAggregateService : IAggregateSpecificOperation<Deal, UpdateIdentity>
    {
        void Update(LegalPersonDeal legalPersonDeal);
    }
}