using System;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders.Copy
{
    // 2+ \BL\Source\API\2Gis.Erm.BLCore.API.Operations\Concrete\Orders
    public interface ICopyOrderOperationService : IOperation<CopyOrderIdentity>
    {
        CopyOrderResult CopyOrder(long orderId, bool isTechnicalTermination);

        CopyOrderResult CopyOrder(long orderId, DateTime beginDistibutionDate, short releaseCountPlan, DiscountType discountType, long newOrderDealId);
    }
}
