using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace DoubleGis.Erm.BLCore.API.OrderValidation
{
    public interface IOrderValidationRuleProvider
    {
        IReadOnlyCollection<OrderValidationRulesContainer> GetAppropriateRules(ValidationType validationType);
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class OrderValidationRulesContainer
    {
        public OrderValidationRulesContainer(
            OrderValidationRuleGroup group,
            bool useCaching,
            bool allRulesScheduled,
            IReadOnlyCollection<OrderValidationRuleDescriptor> ruleDescriptors)
        {
            Group = group;
            UseCaching = useCaching;
            AllRulesScheduled = allRulesScheduled;
            RuleDescriptors = ruleDescriptors;
        }

        public OrderValidationRuleGroup Group { get; private set; }
        public bool UseCaching { get; private set; }
        public bool AllRulesScheduled { get; private set; }
        public IReadOnlyCollection<OrderValidationRuleDescriptor> RuleDescriptors { get; private set; }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public sealed class OrderValidationRuleDescriptor
    {
        public OrderValidationRuleDescriptor(IOrderValidationRule rule, int ruleCode, bool useCaching)
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
