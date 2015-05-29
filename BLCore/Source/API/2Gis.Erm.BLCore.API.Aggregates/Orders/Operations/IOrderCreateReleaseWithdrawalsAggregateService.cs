using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel.DTO;
using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations
{
    public interface IOrderCreateReleaseWithdrawalsAggregateService : IAggregateSpecificService<Order, CreateIdentity>
    {
        void Create(IEnumerable<OrderReleaseWithdrawalDto> releaseWithdrawals);
    }
}