using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations
{
    // TODO {y.baranihin, 31.10.2014}: перенести в BL
    public interface IUpdateLimitAggregateService : IAggregateSpecificOperation<Account, UpdateIdentity>
    {
        void Update(Limit limit, long accountOwnerCode);
    }
}