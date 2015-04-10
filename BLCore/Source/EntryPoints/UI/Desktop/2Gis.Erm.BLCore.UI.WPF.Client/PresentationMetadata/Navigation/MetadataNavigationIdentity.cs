using System;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Kinds;

using MetadataBuilder = NuClear.Metamodeling.Elements.Identities.Builder.Metadata;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Navigation
{
    public sealed class MetadataNavigationIdentity : MetadataKindIdentityBase<MetadataNavigationIdentity>
    {
        private readonly Uri _id = MetadataBuilder.Id.For(MetadataBuilder.Id.DefaultRoot, "UI/Navigation");

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
