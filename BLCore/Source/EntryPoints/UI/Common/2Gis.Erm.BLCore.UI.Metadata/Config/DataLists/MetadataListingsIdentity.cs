using System;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Kinds;

using MetadataBuilder = NuClear.Metamodeling.Elements.Identities.Builder.Metadata;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.DataLists
{
    public sealed class MetadataListingsIdentity : MetadataKindIdentityBase<MetadataListingsIdentity>
    {
        private readonly Uri _id = MetadataBuilder.Id.For(MetadataBuilder.Id.DefaultRoot, "Listings");

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
