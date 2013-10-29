using DoubleGis.Erm.Platform.Web.Mvc.Attributes;

namespace DoubleGis.Erm.Platform.Web.Mvc.ViewModel
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
    }
}
