using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.OrderValidation
{
    public interface IOrderValidationOnStateChangeService
    {
        ValidateOrdersResult Validate(long orderId, OrderState newState, OrderValidationPredicate orderValidationPredicate, ValidateOrdersRequest validateOrdersRequest);
    }
}
