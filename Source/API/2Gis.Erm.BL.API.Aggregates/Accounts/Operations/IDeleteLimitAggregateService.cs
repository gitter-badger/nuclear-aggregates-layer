using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BL.API.Aggregates.Accounts.Operations
{
    public interface IDeleteLimitAggregateService : IAggregateSpecificOperation<Account, DeleteIdentity>
    {
        void Delete(Limit limit);
    }
}