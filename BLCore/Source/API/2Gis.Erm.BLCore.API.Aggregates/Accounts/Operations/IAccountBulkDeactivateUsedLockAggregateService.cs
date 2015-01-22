using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.DTO;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations
{
    public interface IAccountBulkDeactivateUsedLockAggregateService : IAggregateSpecificOperation<Account, BulkDeactivateIdentity>
    {
        void Deactivate(
            IEnumerable<DeactivateLockDto> lockInfos,
            IReadOnlyDictionary<long, long> debitAccountDetailsMap);
    }
}