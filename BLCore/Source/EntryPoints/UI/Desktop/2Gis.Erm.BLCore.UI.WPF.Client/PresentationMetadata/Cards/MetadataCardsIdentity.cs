using System;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Kinds;

using MetadataBuilder = NuClear.Metamodeling.Elements.Identities.Builder.Metadata;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Cards
{
    public sealed class MetadataCardsIdentity : MetadataKindIdentityBase<MetadataCardsIdentity>
    {
        private readonly Uri _id = MetadataBuilder.Id.For(MetadataBuilder.Id.DefaultRoot, "UI/Cards");

        public override Uri Id
        {
            get { return _id; }
        }

        public override string Description
        {
            get { return "Erm entity cards descriptive metadata"; }
        }
    }
}
