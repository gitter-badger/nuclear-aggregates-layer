using DoubleGis.Erm.Platform.Model.Metadata.Common;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Documents
{
    public interface IReferencedElementStructure : IDocumentElementStructure
    {
        bool IsReferenceEvaluated { get; }
        IConfigElement ReferencedElement { get; }
        IConfigElementIdentity ReferencedElementIdentity { get; }
    }
}