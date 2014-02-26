using System;

using DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.Discounts;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Orders.Discounts
{
    public sealed class RecalculateOrderDiscountHandler : RequestHandler<RecalculateOrderDiscountRequest, RecalculateOrderDiscountResponse>
    {

        private readonly IOrderReadModel _orderReadModel;

        public RecalculateOrderDiscountHandler(IOrderReadModel orderReadModel)
        {
            _orderReadModel = orderReadModel;
        }

        protected override RecalculateOrderDiscountResponse Handle(RecalculateOrderDiscountRequest request)
        {
            var response = new RecalculateOrderDiscountResponse();

            if (request.OrderId == 0 || request.OrderType == OrderType.SelfAds)
            {
                return response;
            }

            if (request.ReleaseCountFact < 1 || request.ReleaseCountFact > 12)
            {
                throw new NotificationException(BLResources.ReleaseCountPlanRangeMessage);
            }

            if (!(0.0m <= request.DiscountPercent && request.DiscountPercent <= 100.0m))
            {
                throw new NotificationException(BLResources.OrderValidateDiscountPercentInvalid);
            }

            if (request.DiscountSum < 0.0m)
            {
                throw new NotificationException(BLResources.OrderValidateDiscountNegative);
            }

            var payablePlanSum = _orderReadModel.GetPayablePlanSum(request.OrderId, request.ReleaseCountFact);

            if (payablePlanSum == 0m)
            {
                return response;
            }

            var exactDiscountSum = request.InPercents ? (payablePlanSum * request.DiscountPercent) / 100m : request.DiscountSum;

            response.CorrectedDiscountSum = Math.Round(exactDiscountSum, 2, MidpointRounding.ToEven);
            response.CorrectedDiscountPercent = (exactDiscountSum * 100m) / payablePlanSum;

            return response;
        }
    }
}
