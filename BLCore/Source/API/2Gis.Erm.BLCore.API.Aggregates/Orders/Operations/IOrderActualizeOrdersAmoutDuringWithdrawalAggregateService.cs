using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals;
using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations
{
    public interface IOrderActualizeOrdersAmoutDuringWithdrawalAggregateService : IAggregatePartService<Order>
    {
        IReadOnlyDictionary<long, Order> Actualize(IEnumerable<ActualizeOrdersDto> orders, bool isWithdrawalReverting);
    }
}