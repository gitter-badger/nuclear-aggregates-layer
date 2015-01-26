using System;

using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Kinds;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Cards
{
    public sealed class MetadataCardsIdentity : MetadataKindIdentityBase<MetadataCardsIdentity>
    {
        private readonly Uri _id = IdBuilder.For("UI/Cards");

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
