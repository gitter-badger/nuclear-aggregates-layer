using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.OrderPositions
{
    public sealed class RecalculateOrderPositionDiscountHandler : RequestHandler<RecalculateOrderPositionDiscountRequest, RecalculateOrderPositionDiscountResponse>
    {
        protected override RecalculateOrderPositionDiscountResponse Handle(RecalculateOrderPositionDiscountRequest request)
        {
            var response = new RecalculateOrderPositionDiscountResponse();

            var payablePriceWithVat = request.PricePerUnitWithVat * request.Amount * request.OrderReleaseCountPlan;
            if (payablePriceWithVat == 0m)
            {
                return response;
            }

            var exactDiscountSum = request.InPercent ? (payablePriceWithVat * request.DiscountPercent) / 100m : request.DiscountSum;
            response.CorrectedDiscountSum = Math.Round(exactDiscountSum, 2, MidpointRounding.ToEven);
            response.CorrectedDiscountPercent = (exactDiscountSum * 100m) / payablePriceWithVat;

            return response;
        }
    }
}
