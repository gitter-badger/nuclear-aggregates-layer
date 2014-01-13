using DoubleGis.Erm.Platform.Model.Metadata.Common;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Documents
{
    public interface IReferenceEvaluator
    {
        bool TryEvaluate(IConfigElement referencedConfigElement);
    }
}