using DoubleGis.Erm.Platform.Web.Mvc.Attributes;

namespace DoubleGis.Erm.Platform.Web.Mvc.ViewModel
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