using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel.DTO;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations
{
    public interface IOrderDeleteReleaseWithdrawalsAggregateService : IAggregateSpecificOperation<Order, DeleteIdentity>
    {
        void Delete(IEnumerable<OrderReleaseWithdrawalDto> releaseWithdrawals);
    }
}