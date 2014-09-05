using System;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities.Concrete;

namespace DoubleGis.Erm.BLCore.API.OrderValidation.Metadata
{
    public sealed class OrderValidationRuleMetadata : MetadataElement<OrderValidationRuleMetadata, OrderValidationRuleMetadataBuilder>
    {
        private readonly Type _ruleType;
        private readonly OrderValidationRuleGroup _ruleGroup;
        private readonly int _ruleCode;
        private readonly MetadataElementIdentity _identity;

        public OrderValidationRuleMetadata(Type ruleType, OrderValidationRuleGroup ruleGroup, int ruleCode)
            : base(Enumerable.Empty<IMetadataFeature>())
        {
            _identity = new MetadataElementIdentity(new Uri(ruleType.Name, UriKind.Relative));
            _ruleType = ruleType;
            _ruleGroup = ruleGroup;
            _ruleCode = ruleCode;
        }

        public override IMetadataElementIdentity Identity
        {
            get { return _identity; }
        }

        public Type RuleType
        {
            get { return _ruleType; }
        }

        public OrderValidationRuleGroup RuleGroup
        {
            get { return _ruleGroup; }
        }

        public int RuleCode
        {
            get { return _ruleCode; }
        }

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return _ruleType.Name;
        }
    }
}
