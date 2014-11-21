using System;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Kinds;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Documents
{
    public sealed class MetadataDocumentsIdentity : MetadataKindIdentityBase<MetadataDocumentsIdentity>
    {
        private readonly Uri _id = IdBuilder.For("UI/Documents");

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
