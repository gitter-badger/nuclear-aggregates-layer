using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Identities.Concrete;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Documents
{
    public sealed class DocumentMetadata : ViewModelMetadata<DocumentMetadata, DocumentMetadataBuilder>
    {
        private readonly MetadataElementIdentity _identity;

        public DocumentMetadata(
            StrictOperationIdentity documentTargetOperation,
            IEnumerable<IMetadataFeature> features)
            : base(features)
        {
            _identity = new MetadataElementIdentity(documentTargetOperation.IdFor<MetadataDocumentsIdentity>());
        }

        public override IMetadataElementIdentity Identity
        {
            get { return _identity; }
        }

        public override void ActualizeId(IMetadataElementIdentity actualMetadataElementIdentity)
        {
            throw new System.NotImplementedException();
        }
    }
}
