﻿using System;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;

namespace DoubleGis.Erm.BLCore.API.OrderValidation.Metadata
{
    public sealed class OrderValidationRuleMetadata : MetadataElement<OrderValidationRuleMetadata, OrderValidationRuleMetadataBuilder>
    {
        private readonly Type _ruleType;
        private readonly int _ruleCode;
        private IMetadataElementIdentity _identity;

        public OrderValidationRuleMetadata(Type ruleType, int ruleCode)
            : base(Enumerable.Empty<IMetadataFeature>())
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
