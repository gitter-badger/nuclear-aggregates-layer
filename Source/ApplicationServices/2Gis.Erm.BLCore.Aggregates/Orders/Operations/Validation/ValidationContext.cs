using DoubleGis.Erm.BLCore.API.OrderValidation;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.Validation
{
    public sealed class ValidationContext
    {
        public OrderValidationRuleGroup OrderValidationRuleGroup { get; private set; }
        public ValidationType ValidationType { get; private set; }

        public ValidationContext(OrderValidationRuleGroup orderValidationRuleGroup, ValidationType validationType)
        {
            OrderValidationRuleGroup = orderValidationRuleGroup;
            ValidationType = validationType;
        }
    }
}