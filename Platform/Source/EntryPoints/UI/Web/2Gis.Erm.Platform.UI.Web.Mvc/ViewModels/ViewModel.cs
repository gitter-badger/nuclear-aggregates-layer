using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;

namespace DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels
{
    public partial class ViewModel : IViewModel
    {
        [PresentationLayerProperty]
        public string Message { get; set; }
        [PresentationLayerProperty]
        public MessageType MessageType { get; set; }
        [PresentationLayerProperty]
        public string EntityStateToken { get; set; }
        [PresentationLayerProperty]
        public bool IsSuccess { get; set; }

        public void SetInfo(string message)
        {
            Message = message;
            MessageType = MessageType.Info;
        }

        public void SetWarning(string message)
        {
            Message = message;
            MessageType = MessageType.Warning;
        }

        public void SetCriticalError(string message)
        {
            Message = message;
            MessageType = MessageType.CriticalError;
        }
    }
}
