using System;
using System.Collections.Generic;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Elements.Identities.Builder;

namespace DoubleGis.Erm.BLCore.API.OrderValidation.Metadata
{
    public sealed class OrderValidationRuleMetadata : MetadataElement<OrderValidationRuleMetadata, OrderValidationRuleMetadataBuilder>
    {
        private readonly Type _ruleType;
        private readonly int _ruleCode;
        private IMetadataElementIdentity _identity;

        public OrderValidationRuleMetadata(Type ruleType, int ruleCode, IEnumerable<IMetadataFeature> features)
            : base(features)
        {
            _identity = new Uri(ruleType.Name, UriKind.Relative).AsIdentity();
            _ruleType = ruleType;
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

        public int RuleCode
        {
            get { return _ruleCode; }
        }

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
            _identity = actualMetadataElementIdentity;
        }

        public override string ToString()
        {
            return _ruleType.Name;
        }
    }
}
