using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
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

        void SetInfo(string message);

        void SetWarning(string message);

        void SetCriticalError(string message);
    }
}