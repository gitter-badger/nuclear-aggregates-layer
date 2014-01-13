using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel
{
    // Это не сервис операции
    // TODO {all, 07.11.2013}: в платформе 2.0 заменить на ReadModel
    [Obsolete]
    public interface IGetPositionsByOrderService
    {
        IEnumerable<long> GetPositionIds(long orderId);
    }
}
