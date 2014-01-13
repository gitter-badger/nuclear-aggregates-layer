using DoubleGis.Erm.Platform.Model.Metadata.Common.Features.Handler;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.ViewModel;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Documents
{
    public interface IAttachedElementStructure : 
        IViewModelStructure, 
        IDocumentElementStructure,
        IHandlerBoundElement
    {
    }
}