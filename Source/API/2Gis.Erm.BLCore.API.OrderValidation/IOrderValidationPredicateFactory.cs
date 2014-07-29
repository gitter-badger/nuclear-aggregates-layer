using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.OrderValidation
{
    public interface IOrderValidationPredicateFactory
    {
        OrderValidationPredicate CreatePredicate(long orderId, out OrderState currentOrderState, out TimePeriod period);
        OrderValidationPredicate CreatePredicate(long organizationUnitId, TimePeriod period, long? ownerCode, bool includeOwnerDescendants);
    }
}