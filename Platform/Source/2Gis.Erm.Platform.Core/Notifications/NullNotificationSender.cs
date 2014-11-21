using DoubleGis.Erm.Platform.API.Core.Notifications;

namespace DoubleGis.Erm.Platform.Core.Notifications
{
    public class NullNotificationSender : INotificationSender
    {
        public void PostMessage(NotificationAddress[] to, string subject, string body)
        {
        }

        public void PostMessage(NotificationAddress[] to, string subject, NotificationBody body)
        {
        }

        public void PostMessage(NotificationAddress sender,
                                NotificationAddress[] to,
                                NotificationAddress[] cc,
                                string subject,
                                string body,
                                bool isBodyHtml,
                                NotificationAttachment[] attachments)
        {
        }
    }
}