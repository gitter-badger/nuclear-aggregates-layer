using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Withdrawals.Operations
{
    public interface IBulkDeleteLockDetailsAggregateService : IAggregateSpecificOperation<Account, BulkDeleteIdentity>
    {
        void Delete(IReadOnlyCollection<LockDto> lockDetailsToDelete);
    }
}