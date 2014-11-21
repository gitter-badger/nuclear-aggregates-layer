using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations
{
    public interface IAccountBulkReopenLimitsAggregateService : IAggregateSpecificOperation<Account, BulkActivateIdentity>
    {
        void Reopen(IEnumerable<Limit> limits);
    }
}