using DoubleGis.Erm.Core.Services.Notifications;

namespace DoubleGis.Erm.BL.Services.Notifications
{
    // 2+: \Platform\Source\2Gis.Erm.Platform.Core\Notifications\NullNotificationSender.cs
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