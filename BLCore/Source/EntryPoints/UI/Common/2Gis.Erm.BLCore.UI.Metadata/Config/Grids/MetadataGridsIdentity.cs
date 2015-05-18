using System;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Kinds;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.Grids
{
    public sealed class MetadataGridsIdentity : MetadataKindIdentityBase<MetadataGridsIdentity>
    {
        private readonly Uri _id = NuClear.Metamodeling.Elements.Identities.Builder.Metadata.Id.For("UI/Grids");

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
