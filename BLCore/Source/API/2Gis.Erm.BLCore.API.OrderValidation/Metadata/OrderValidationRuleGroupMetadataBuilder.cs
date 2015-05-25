using DoubleGis.Erm.BLCore.API.OrderValidation.Metadata.Features;

using NuClear.Metamodeling.Elements;

namespace DoubleGis.Erm.BLCore.API.OrderValidation.Metadata
{
    public sealed class OrderValidationRuleGroupMetadataBuilder : MetadataElementBuilder<OrderValidationRuleGroupMetadataBuilder, OrderValidationRuleGroupMetadata>
    {
        private OrderValidationRuleGroup _ruleGroup;
        
        public OrderValidationRuleGroupMetadataBuilder UseCaching
        {
            get
            {
                AddFeatures(new UseCachingFeature());
                return this;
            }
        }

        public OrderValidationRuleGroupMetadataBuilder Group(OrderValidationRuleGroup ruleGroup)
        {
            _ruleGroup = ruleGroup;
            return this;
        }

        public OrderValidationRuleGroupMetadataBuilder Rules(params OrderValidationRuleMetadata[] rules)
        {
            Childs(rules);
            return this;
        }

        public OrderValidationRuleGroupMetadataBuilder EnableCachingFor(ValidationType validationType)
        {
            AddFeatures(new EnableCachingForValidationTypeFeature(validationType));
            return this;
        }

        protected override OrderValidationRuleGroupMetadata Create()
        {
            return new OrderValidationRuleGroupMetadata(_ruleGroup, Features);
        }
    }
}