using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.DTO;
using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations
{
    public interface IAccountBulkDeleteLocksAggregateService : IAggregateSpecificOperation<Account, BulkDeleteIdentity>
    {
        void Delete(IEnumerable<LockDto> locks);
    }
}