using DoubleGis.Erm.BLCore.API.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.OrderPositions
{
    public sealed class GetInitialDiscountHandler : RequestHandler<GetInitialDiscountRequest, GetInitialDiscountResponse>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderReadModel _orderReadModel;

        public GetInitialDiscountHandler(IOrderRepository orderRepository, IOrderReadModel orderReadModel)
        {
            _orderRepository = orderRepository;
            _orderReadModel = orderReadModel;
        }

        protected override GetInitialDiscountResponse Handle(GetInitialDiscountRequest request)
        {
            var orderDiscounts = _orderReadModel.GetOrderDiscounts(request.OrderId);
            var response = new GetInitialDiscountResponse
                {
                    DiscountPercent = orderDiscounts.CalculateDiscountViaPercent ? orderDiscounts.DiscountPercent : 0M
                };

            return response;
        }
    }
}
