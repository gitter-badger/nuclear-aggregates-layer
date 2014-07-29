using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.API.OrderValidation
{
    public interface IOrderValidationRule
    {
        IReadOnlyList<OrderValidationMessage> Validate(OrderValidationPredicate filterPredicate, IEnumerable<long> invalidOrderIds, ValidateOrdersRequest request);
    }
}