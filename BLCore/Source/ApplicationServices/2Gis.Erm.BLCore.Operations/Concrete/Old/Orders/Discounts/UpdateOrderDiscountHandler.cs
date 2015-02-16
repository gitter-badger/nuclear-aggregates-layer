using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.Discounts;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Orders.Discounts
{
    public sealed class UpdateOrderDiscountHandler : RequestHandler<UpdateOrderDiscountRequest, EmptyResponse>
    {
        private readonly IOrderReadModel _orderReadModel;
        private readonly IPublicService _publicService;

        public UpdateOrderDiscountHandler(IOrderReadModel orderReadModel, IPublicService publicService)
        {
            _orderReadModel = orderReadModel;
            _publicService = publicService;
        }

        protected override EmptyResponse Handle(UpdateOrderDiscountRequest request)
        {
            var order = request.Order;

            var originalOrderInfo = _orderReadModel.GetFinancialInformation(order.Id);

            // если скидка не поменялась и число выпусков такое же, то и пересчитывать нечего
            var discountNotChanged = originalOrderInfo.DiscountSum == order.DiscountSum;
            var discountInPercentnotChanged = originalOrderInfo.DiscountInPercent == request.DiscountInPercents;
            var releaseCountFactNotChanged = originalOrderInfo.ReleaseCountFact == order.ReleaseCountFact;
            if (discountNotChanged && discountInPercentnotChanged && releaseCountFactNotChanged)
            {
                return Response.Empty;
            }

            // пересчитаем скидку, если на карточке подхимичили с ней в postback (режим паранойи)
            var recalculateResponse = (RecalculateOrderDiscountResponse)_publicService.Handle(new RecalculateOrderDiscountRequest
                                                                                                  {
                                                                                                      OrderId = order.Id,
                                                                                                      ReleaseCountFact = order.ReleaseCountFact,

                                                                                                      InPercents = request.DiscountInPercents,
                                                                                                      DiscountSum = order.DiscountSum ?? 0m,
                                                                                                      DiscountPercent = order.DiscountPercent ?? 0m,
                                                                                                  });

            order.DiscountSum = recalculateResponse.CorrectedDiscountSum;
            order.DiscountPercent = recalculateResponse.CorrectedDiscountPercent;

            _publicService.Handle(new UpdateOrderFinancialPerformanceRequest
                                      {
                                          Order = order,
                                          ReleaseCountFact = order.ReleaseCountFact,

                                          RecalculateFromOrder = true,
                                          OrderDiscountInPercents = request.DiscountInPercents,
                                      });

            return Response.Empty;
        }
    }
}
