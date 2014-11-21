using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BL.API.Aggregates.Accounts.Operations
{    
    public interface ICreateLimitAggregateService : IAggregateSpecificOperation<Account, CreateIdentity>
    {
        void Create(Limit limit, long accountOwnerCode);
    }
}