using System;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Kinds;

namespace DoubleGis.Erm.BLCore.API.OrderValidation.Metadata
{
    public sealed class MetadataOrderValidationRulesIdentity : MetadataKindIdentityBase<MetadataOrderValidationRulesIdentity>
    {
        private readonly Uri _id = IdBuilder.For("OrderValidation/Rules");

        public override Uri Id
        {
            get { return _id; }
        }

        public override string Description
        {
            get { return "Erm order validation rules descriptive metadata"; }
        }
    }
}
