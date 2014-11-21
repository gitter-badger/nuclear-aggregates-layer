namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Models.Administration
{
    public sealed class MessageQueueManagementViewModel
    {
        public bool HasAccessToWithdrawal { get; set; }
        public bool HasAccessToRelease { get; set; }
        public bool HasAccessToCorporateQueue { get; set; }
    }
}