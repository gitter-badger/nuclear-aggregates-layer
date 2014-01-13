using System;
using System.Collections.Generic;
using System.Linq;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel
{
    [Obsolete]
    public class GetPositionsByOrderService : IGetPositionsByOrderService
    {
        private IOrderRepository _orderRepository;

        public GetPositionsByOrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public IEnumerable<long> GetPositionIds(long orderId)
        {
            var orderInfo = _orderRepository.GetOrderForProlongationInfo(orderId);
            return orderInfo.Positions.Select(x => x.PositionId).ToArray();
        }
    }
}
