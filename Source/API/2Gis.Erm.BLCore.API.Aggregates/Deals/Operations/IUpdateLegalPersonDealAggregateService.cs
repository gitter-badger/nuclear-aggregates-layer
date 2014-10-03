using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Deals.Operations
{
    public interface IUpdateLegalPersonDealAggregateService : IAggregateSpecificOperation<Deal, UpdateIdentity>
    {
        void Update(LegalPersonDeal legalPersonDeal);
    }
}