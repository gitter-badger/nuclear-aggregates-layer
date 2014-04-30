using System;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Kinds;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Navigation
{
    public sealed class MetadataNavigationIdentity : MetadataKindIdentityBase<MetadataNavigationIdentity>
    {
        private readonly Uri _id = IdBuilder.For("UI/Navigation");

        public override Uri Id
        {
            get { return _id; }
        }

        public override string Description
        {
            get { return "Erm UI navigation pane descriptive metadata"; }
        }
    }
}
