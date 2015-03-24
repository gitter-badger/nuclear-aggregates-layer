using System;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Kinds;

namespace DoubleGis.Erm.BLCore.API.OrderValidation.Metadata
{
    public sealed class MetadataOrderValidationIdentity : MetadataKindIdentityBase<MetadataOrderValidationIdentity>
    {
        private readonly Uri _id = NuClear.Metamodeling.Elements.Identities.Builder.Metadata.Id.For("OrderValidation").Build();

        public override Uri Id
        {
            get { return _id; }
        }

        public override string Description
        {
            get { return "Erm order validation subsystem descriptive metadata"; }
        }
    }
}
