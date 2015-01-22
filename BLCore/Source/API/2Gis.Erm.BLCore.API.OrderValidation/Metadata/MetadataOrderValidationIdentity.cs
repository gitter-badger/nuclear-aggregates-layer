using System;

using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Kinds;

namespace DoubleGis.Erm.BLCore.API.OrderValidation.Metadata
{
    public sealed class MetadataOrderValidationIdentity : MetadataKindIdentityBase<MetadataOrderValidationIdentity>
    {
        private readonly Uri _id = IdBuilder.For("OrderValidation");

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
