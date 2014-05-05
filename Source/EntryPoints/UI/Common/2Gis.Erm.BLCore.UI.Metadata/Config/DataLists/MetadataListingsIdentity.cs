using System;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Kinds;

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
