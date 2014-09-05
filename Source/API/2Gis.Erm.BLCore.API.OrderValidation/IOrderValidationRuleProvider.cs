using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.API.OrderValidation
{
    public interface IOrderValidationRuleProvider
    {
        IEnumerable<OrderValidationRulesContianer> GetAppropriateRules(ValidationType validationType);
    }

    public sealed class OrderValidationRulesContianer
    {
        public OrderValidationRulesContianer(OrderValidationRuleGroup group, IReadOnlyCollection<OrderValidationRuleDescritpor> ruleDescriptors)
        {
            Group = group;
            RuleDescriptors = ruleDescriptors;
        }

        public OrderValidationRuleGroup Group { get; private set; }
        public IReadOnlyCollection<OrderValidationRuleDescritpor> RuleDescriptors { get; private set; }
    }

    public sealed class OrderValidationRuleDescritpor
    {
        public OrderValidationRuleDescritpor(IOrderValidationRule rule, int ruleCode, bool cachingExplicitlyDisabled)
        {
            RuleCode = ruleCode;
            Rule = rule;
            CachingExplicitlyDisabled = cachingExplicitlyDisabled;
        }

        public IOrderValidationRule Rule { get; private set; }
        public int RuleCode { get; private set; }
        public bool CachingExplicitlyDisabled { get; private set; }
    }
}
