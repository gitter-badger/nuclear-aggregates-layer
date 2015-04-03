using System;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Kinds;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Documents
{
    public sealed class MetadataDocumentsIdentity : MetadataKindIdentityBase<MetadataDocumentsIdentity>
    {
        private readonly Uri _id = NuClear.Metamodeling.Elements.Identities.Builder.Metadata.Id.For("UI/Documents");

        public override Uri Id
        {
            get { return _id; }
        }

        public override string Description
        {
            get { return "Erm usecases step presentation document descriptive metadata"; }
        }
    }
}
