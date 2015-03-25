using System.Collections.Generic;

using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel;

using NuClear.Metamodeling.Domain.Elements.Identities.Builder;
using NuClear.Metamodeling.Elements.Aspects.Features;
using NuClear.Metamodeling.Elements.Identities;
using NuClear.Metamodeling.Elements.Identities.Concrete;
using NuClear.Model.Common.Operations.Identity;

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
