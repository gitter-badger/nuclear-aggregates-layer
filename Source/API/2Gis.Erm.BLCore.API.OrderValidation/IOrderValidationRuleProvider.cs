using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.API.OrderValidation
{
    public interface IOrderValidationRuleProvider
    {
        IEnumerable<OrderValidationRuleContainer> GetAppropriateRules(ValidationType validationType);
    }

    public sealed class OrderValidationRuleContainer
    {
        public OrderValidationRuleContainer(OrderValidationRuleGroup group, IEnumerable<IOrderValidationRule> rules)
        {
            Group = group;
            Rules = rules;
        }

        public OrderValidationRuleGroup Group { get; private set; }
        public IEnumerable<IOrderValidationRule> Rules { get; private set; }
    }
}
