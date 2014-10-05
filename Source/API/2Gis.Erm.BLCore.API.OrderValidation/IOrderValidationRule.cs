using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.API.OrderValidation
{
    public interface IOrderValidationRule
    {
        IEnumerable<OrderValidationMessage> Validate(ValidationParams validationParams, OrderValidationPredicate combinedPredicate, IValidationResultsBrowser validationResultsBrowser);
    }
}