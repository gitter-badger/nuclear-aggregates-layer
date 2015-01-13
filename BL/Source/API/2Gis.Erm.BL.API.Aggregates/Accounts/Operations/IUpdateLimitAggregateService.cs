using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BL.API.Aggregates.Accounts.Operations
{
    public interface IUpdateLimitAggregateService : IAggregateSpecificOperation<Account, UpdateIdentity>
    {
        // TODO {all, 17.12.2014}: accountOwnerCode должен проставлять вызывающий код, нет очевидного инварианта, когда update влечет обязательную смену куратора
        void Update(Limit limit, long accountOwnerCode);
    }
}