using System.Linq;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Operations;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Documents
{
    public sealed class DocumentMetadataBuilder : ViewModelMetadataBuilder<DocumentMetadataBuilder, DocumentMetadata>
    {
        protected override DocumentMetadata Create()
        {
            var targetOperation = Features.OfType<OperationFeature>().Single().Identity;
            return new DocumentMetadata(targetOperation, Features);
        }
    }
}
