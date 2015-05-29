using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BL.API.Aggregates.Accounts.Operations
{    
    public interface ICreateLimitAggregateService : IAggregateSpecificService<Account, CreateIdentity>
    {
        void Create(Limit limit, long accountOwnerCode);
    }
}