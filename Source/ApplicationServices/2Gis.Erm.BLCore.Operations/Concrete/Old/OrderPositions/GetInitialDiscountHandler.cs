using DoubleGis.Erm.BLCore.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.OrderPositions
{
    public sealed class GetInitialDiscountHandler : RequestHandler<GetInitialDiscountRequest, GetInitialDiscountResponse>
    {
        private readonly IOrderRepository _orderRepository;

        public GetInitialDiscountHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        protected override GetInitialDiscountResponse Handle(GetInitialDiscountRequest request)
        {
            var orderDiscounts = _orderRepository.GetOrderDiscounts(request.OrderId);
            var response = new GetInitialDiscountResponse
                {
                    DiscountPercent = orderDiscounts.CalculateDiscountViaPercent ? orderDiscounts.DiscountPercent : 0M
                };

            return response;
        }
    }
}
