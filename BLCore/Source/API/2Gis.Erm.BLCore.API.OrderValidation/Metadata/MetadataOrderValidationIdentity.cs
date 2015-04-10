using System;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Kinds;

using MetadataBuilder = NuClear.Metamodeling.Elements.Identities.Builder.Metadata;

namespace DoubleGis.Erm.BLCore.API.OrderValidation.Metadata
{
    public sealed class MetadataOrderValidationIdentity : MetadataKindIdentityBase<MetadataOrderValidationIdentity>
    {
        private readonly Uri _id = MetadataBuilder.Id.For(MetadataBuilder.Id.DefaultRoot, "OrderValidation");

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
