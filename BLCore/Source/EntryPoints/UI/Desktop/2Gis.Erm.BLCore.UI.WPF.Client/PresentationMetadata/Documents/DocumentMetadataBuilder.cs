using System.Linq;

using NuClear.Metamodeling.UI.Elements.Aspects.Features.Operations;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Documents
{
    public sealed class DocumentMetadataBuilder : ViewModelMetadataBuilder<DocumentMetadataBuilder, DocumentMetadata>
    {
        protected override DocumentMetadata Create()
        {
            var operationsSet = Features.OfType<OperationsSetFeature>().Single();
            var targetOperation = operationsSet.OperationFeatures.Select(f => f.Identity).Single();
            return new DocumentMetadata(targetOperation, Features);
        }
    }
}
