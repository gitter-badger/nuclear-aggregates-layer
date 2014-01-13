using DoubleGis.Erm.BLCore.Aggregates.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.OrderPositions
{
    public sealed class ViewOrderPositionHandler : RequestHandler<ViewOrderPositionRequest, ViewOrderPositionResponse>
    {
        private readonly IOrderRepository _orderRepository;

        public ViewOrderPositionHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        protected override ViewOrderPositionResponse Handle(ViewOrderPositionRequest request)
        {
            var positionInfo = _orderRepository.GetOrderPositionDetailedInfo(request.OrderPositionId, request.OrderId, request.PricePositionId, request.IncludeHidden);

            return new ViewOrderPositionResponse
            {
                OrderReleaseCountFact = positionInfo.ReleaseCountFact,
                OrderReleaseCountPlan = positionInfo.ReleaseCountPlan,
                PlatformName = positionInfo.Platform,
                PricePositionAmount = positionInfo.Amount,
                AmountSpecificationMode = positionInfo.AmountSpecificationMode,
                PricePositionCost = positionInfo.PricePositionCost,
                IsPositionComposite = positionInfo.IsComposite,
                IsBudget = positionInfo.IsBudget,
                LinkingObjectsSchema = positionInfo.LinkingObjectsSchema,
                PricePerUnit = positionInfo.PricePerUnit,
                VatRatio = positionInfo.VatRatio,
            };
        }
    }
}
