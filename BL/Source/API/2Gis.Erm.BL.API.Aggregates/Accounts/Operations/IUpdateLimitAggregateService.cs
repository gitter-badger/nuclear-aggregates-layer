using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BL.API.Aggregates.Accounts.Operations
{
    public interface IUpdateLimitAggregateService : IAggregateSpecificService<Account, UpdateIdentity>
    {
        // TODO {all, 17.12.2014}: accountOwnerCode должен проставлять вызывающий код, нет очевидного инварианта, когда update влечет обязательную смену куратора
        void Update(Limit limit, long accountOwnerCode);
    }
}