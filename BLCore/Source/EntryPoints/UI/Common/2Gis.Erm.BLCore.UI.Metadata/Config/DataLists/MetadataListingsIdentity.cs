using System;

using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Kinds;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.DataLists
{
    public sealed class MetadataListingsIdentity : MetadataKindIdentityBase<MetadataListingsIdentity>
    {
        private readonly Uri _id = IdBuilder.For("Listings");

        public override Uri Id
        {
            get { return _id; }
        }

        public override string Description
        {
            get { return "Erm listings descriptive metadata"; }
        }
    }
}
