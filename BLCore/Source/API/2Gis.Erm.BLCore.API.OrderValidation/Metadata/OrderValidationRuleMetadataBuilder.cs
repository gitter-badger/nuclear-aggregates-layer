using System;

using DoubleGis.Erm.BLCore.API.OrderValidation.Metadata.Features;
using DoubleGis.Erm.Platform.Model;
using NuClear.Metamodeling.Elements;

namespace DoubleGis.Erm.BLCore.API.OrderValidation.Metadata
{
    public sealed class OrderValidationRuleMetadataBuilder : MetadataElementBuilder<OrderValidationRuleMetadataBuilder, OrderValidationRuleMetadata>
    {
        private Type _ruleType;
        private int _ruleCode;

        public OrderValidationRuleMetadataBuilder NonManual
        {
            get
            {
                AddFeatures(new NonManualRuleFeature());
                return this;
            }
        }

        public OrderValidationRuleMetadataBuilder Common
        {
            get
            {
                AddFeatures(new CommonRuleFeature());
                return this;
            }
        }

        public OrderValidationRuleMetadataBuilder SingleOrderValidation
        {
            get
            {
                AddFeatures(new SingleOrderValidationRuleFeature());
                return this;
            }
        }

        public OrderValidationRuleMetadataBuilder Rule<TOrderValidationRule>(int ruleCode)
            where TOrderValidationRule : class, IOrderValidationRule
        {
            _ruleType = typeof(TOrderValidationRule);
            _ruleCode = ruleCode;
            return this;
        }

        public OrderValidationRuleMetadataBuilder DisableFor(BusinessModel businessModel)
        {
            AddFeatures(new DisabledForBusinessModelFeature(businessModel));
            return this;
        }

        public OrderValidationRuleMetadataBuilder AvailableFor(ValidationType validationType)
        {
            AddFeatures(new AvailableForValidationTypeFeature(validationType));
            return this;
        }

        protected override OrderValidationRuleMetadata Create()
        {
            return new OrderValidationRuleMetadata(_ruleType, _ruleCode, Features);
        }
    }
}