namespace DoubleGis.Erm.Platform.Web.Mvc.ViewModel
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