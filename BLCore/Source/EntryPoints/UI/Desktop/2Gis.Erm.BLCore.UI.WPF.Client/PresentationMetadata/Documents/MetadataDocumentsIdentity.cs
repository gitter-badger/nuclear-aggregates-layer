using System;

using NuClear.Metamodeling.Elements.Identities.Builder;
using NuClear.Metamodeling.Kinds;

using MetadataBuilder = NuClear.Metamodeling.Elements.Identities.Builder.Metadata;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Documents
{
    public sealed class MetadataDocumentsIdentity : MetadataKindIdentityBase<MetadataDocumentsIdentity>
    {
        private readonly Uri _id = MetadataBuilder.Id.For(MetadataBuilder.Id.DefaultRoot, "UI/Documents");

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
