using System.Linq;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;

namespace DoubleGis.Erm.BLCore.API.OrderValidation.Metadata
{
    public sealed class OrderValidationRuleGroupMetadata : MetadataElement<OrderValidationRuleGroupMetadata, OrderValidationRuleGroupMetadataBuilder>
    {
        private readonly OrderValidationRuleGroup _ruleGroup;
        private IMetadataElementIdentity _identity;

        public OrderValidationRuleGroupMetadata(OrderValidationRuleGroup ruleGroup)
            : base(Enumerable.Empty<IMetadataFeature>())
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