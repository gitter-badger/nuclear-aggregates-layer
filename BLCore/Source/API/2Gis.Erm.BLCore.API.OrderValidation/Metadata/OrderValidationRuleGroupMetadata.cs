using System.Collections.Generic;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Elements.Identities.Builder;

namespace DoubleGis.Erm.BLCore.API.OrderValidation.Metadata
{
    public sealed class OrderValidationRuleGroupMetadata : MetadataElement<OrderValidationRuleGroupMetadata, OrderValidationRuleGroupMetadataBuilder>
    {
        private readonly OrderValidationRuleGroup _ruleGroup;
        private IMetadataElementIdentity _identity;

        public OrderValidationRuleGroupMetadata(OrderValidationRuleGroup ruleGroup, IEnumerable<IMetadataFeature> features)
            : base(features)
        {
            _identity = NuClear.Metamodeling.Elements.Identities.Builder.Metadata.Id.For<MetadataOrderValidationIdentity>("Rules", ruleGroup.ToString()).Build().AsIdentity();
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