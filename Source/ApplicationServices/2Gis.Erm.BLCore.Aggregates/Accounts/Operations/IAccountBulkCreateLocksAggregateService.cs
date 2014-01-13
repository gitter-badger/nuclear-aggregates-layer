using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.Orders.DTO;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Accounts.Operations
{
    public interface IAccountBulkCreateLocksAggregateService : IAggregateSpecificOperation<Account, BulkCreateIdentity>
    {
        void Create(TimePeriod period, IEnumerable<OrderReleaseInfo> orderReleaseInfo);
    }
}