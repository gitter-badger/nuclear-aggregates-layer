using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.API.OrderValidation
{
    public interface IOrderValidationRuleProvider
    {
        IEnumerable<OrderValidationRulesContianer> GetAppropriateRules(ValidationType validationType);
    }

    public sealed class OrderValidationRulesContianer
    {
        public OrderValidationRulesContianer(
            OrderValidationRuleGroup group,
            bool useCaching,
            bool allRulesScheduled,
            IReadOnlyCollection<OrderValidationRuleDescritpor> ruleDescriptors)
        {
            Group = group;
            UseCaching = useCaching;
            AllRulesScheduled = allRulesScheduled;
            RuleDescriptors = ruleDescriptors;
        }

        public OrderValidationRuleGroup Group { get; private set; }
        public bool UseCaching { get; private set; }
        public bool AllRulesScheduled { get; private set; }
        public IReadOnlyCollection<OrderValidationRuleDescritpor> RuleDescriptors { get; private set; }
    }

    public sealed class OrderValidationRuleDescritpor
    {
        public OrderValidationRuleDescritpor(IOrderValidationRule rule, int ruleCode, bool useCaching)
        {
            RuleCode = ruleCode;
            Rule = rule;
            UseCaching = useCaching;
        }

        public IOrderValidationRule Rule { get; private set; }
        public int RuleCode { get; private set; }
        public bool UseCaching { get; private set; }
    }
}
