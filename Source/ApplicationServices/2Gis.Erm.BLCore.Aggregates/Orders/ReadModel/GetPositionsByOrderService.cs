using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel
{
    [Obsolete]
    public class GetPositionsByOrderService : IGetPositionsByOrderService
    {
        private readonly IOrderReadModel _orderReadModel;

        public GetPositionsByOrderService(IOrderReadModel orderReadModel)
        {
            _orderReadModel = orderReadModel;
        }

        public IEnumerable<long> GetPositionIds(long orderId)
        {
            var orderInfo = _orderReadModel.GetOrderForProlongationInfo(orderId);
            return orderInfo.Positions.Select(x => x.PositionId).ToArray();
        }
    }
}
