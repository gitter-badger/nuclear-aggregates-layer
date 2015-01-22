using System.Collections.Generic;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;

namespace DoubleGis.Erm.BLCore.API.OrderValidation.Metadata
{
    public sealed class OrderValidationRuleGroupMetadata : MetadataElement<OrderValidationRuleGroupMetadata, OrderValidationRuleGroupMetadataBuilder>
    {
        private readonly OrderValidationRuleGroup _ruleGroup;
        private IMetadataElementIdentity _identity;

        public OrderValidationRuleGroupMetadata(OrderValidationRuleGroup ruleGroup, IEnumerable<IMetadataFeature> features)
            : base(features)
        {
            _identity = IdBuilder.For<MetadataOrderValidationIdentity>("Rules", ruleGroup.ToString()).AsIdentity();
            _ruleGroup = ruleGroup;
        }

        public override IMetadataElementIdentity Identity
        {
            get { return _identity; }
        }

        public OrderValidationRuleGroup RuleGroup
        {
            get { return _ruleGroup; }
        }

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
            _identity = actualMetadataElementIdentity;
        }

        public override string ToString()
        {
            return _ruleGroup.ToString();
        }
    }
}