using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.Accounts.DTO;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Accounts.Operations
{
    public interface IAccountBulkActivateLocksAggregateService : IAggregateSpecificOperation<Account, BulkActivateIdentity>
    {
        void Activate(IEnumerable<ActivateLockDto> lockInfos);
    }
}