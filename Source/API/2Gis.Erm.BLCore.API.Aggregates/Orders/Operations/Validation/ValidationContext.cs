using DoubleGis.Erm.BLCore.API.OrderValidation;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.Validation
{
    public sealed class ValidationContext
    {
        public ValidationContext(OrderValidationRuleGroup orderValidationRuleGroup, ValidationType validationType)
        {
            OrderValidationRuleGroup = orderValidationRuleGroup;
            ValidationType = validationType;
        }

        public OrderValidationRuleGroup OrderValidationRuleGroup { get; private set; }
        public ValidationType ValidationType { get; private set; }
    }
}