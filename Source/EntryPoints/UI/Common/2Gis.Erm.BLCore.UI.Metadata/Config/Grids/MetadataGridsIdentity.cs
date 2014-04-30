using System;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Kinds;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.Grids
{
    public sealed class MetadataGridsIdentity : MetadataKindIdentityBase<MetadataGridsIdentity>
    {
        private readonly Uri _id = IdBuilder.For("UI/Grids");

        public override Uri Id
        {
            get { return _id; }
        }

        public override string Description
        {
            get { return "Erm grids descriptive metadata"; }
        }
    }
}
