using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.Accounts.DTO;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Accounts.Operations
{
    public interface IAccountBulkDeactivateUsedLockAggregateService : IAggregateSpecificOperation<Account, BulkDeactivateIdentity>
    {
        void Deactivate(
            IEnumerable<DeactivateLockDto> lockInfos,
            IReadOnlyDictionary<long, long> debitAccountDetailsMap);
    }
}