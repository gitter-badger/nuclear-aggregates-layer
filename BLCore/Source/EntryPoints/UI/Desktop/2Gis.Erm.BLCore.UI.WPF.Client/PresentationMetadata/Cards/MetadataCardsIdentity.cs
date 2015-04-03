using System;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Kinds;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Cards
{
    public sealed class MetadataCardsIdentity : MetadataKindIdentityBase<MetadataCardsIdentity>
    {
        private readonly Uri _id = NuClear.Metamodeling.Elements.Identities.Builder.Metadata.Id.For("UI/Cards");

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
