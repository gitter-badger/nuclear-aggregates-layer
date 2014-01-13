using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.Operations
{
    public interface IOrderActualizeOrdersAmoutDuringWithdrawalAggregateService : IAggregatePartRepository<Order>
    {
        IReadOnlyDictionary<long, Order> Actualize(IEnumerable<ActualizeOrdersDto> orders, bool isWithdrawalReverting);
    }
}