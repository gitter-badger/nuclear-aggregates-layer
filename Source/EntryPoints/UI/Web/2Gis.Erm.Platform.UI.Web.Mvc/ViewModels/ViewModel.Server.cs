namespace DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels
{
    public partial class ViewModel
    {
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