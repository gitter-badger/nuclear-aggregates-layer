using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;

namespace DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels
{
    public interface IViewModel
    {
        [PresentationLayerProperty]
        string Message { get; set; }
        [PresentationLayerProperty]
        MessageType MessageType { get; set; }
        [PresentationLayerProperty]
        string EntityStateToken { get; set; }
        [PresentationLayerProperty]
        bool IsSuccess { get; set; }
    }
}