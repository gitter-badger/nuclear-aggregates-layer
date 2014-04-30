using System;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Kinds;

namespace DoubleGis.Erm.Platform.Model.Metadata.Entities
{
    public sealed class MetadataEntitiesIdentity : MetadataKindIdentityBase<MetadataEntitiesIdentity>
    {
        private readonly Uri _id = IdBuilder.For("Entities");

        public override Uri Id
        {
            get { return _id; }
        }

        public override string Description
        {
            get { return "Erm entity descriptive metadata"; }
        }
    }
}
