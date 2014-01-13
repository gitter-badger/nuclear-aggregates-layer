using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Deals.Operations
{
    public interface IDeleteAfterSaleServiceAggregateService : IAggregateSpecificOperation<Deal, DeleteIdentity>
    {
        void Delete(AfterSaleServiceActivity activity);
    }
}