using System.Collections.Generic;

using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations
{
    public interface IAccountBulkCloseLimitsAggregateService : IAggregateSpecificOperation<Account, BulkDeactivateIdentity>
    {
        void Close(IEnumerable<Limit> limits);
    }
}