using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations
{
    public interface IAccountBulkCreateLocksAggregateService : IAggregateSpecificOperation<Account, BulkCreateIdentity>
    {
        void Create(TimePeriod period, IEnumerable<OrderReleaseInfo> orderReleaseInfo);
    }
}