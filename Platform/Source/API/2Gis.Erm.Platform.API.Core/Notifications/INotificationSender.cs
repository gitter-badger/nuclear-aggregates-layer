using System;

using DoubleGis.Erm.Platform.Model.Simplified;

namespace DoubleGis.Erm.Platform.API.Core.Notifications
{
    public interface INotificationSender : ISimplifiedModelConsumer
    {
        void PostMessage(NotificationAddress[] to, String subject, String body);
        void PostMessage(NotificationAddress[] to, String subject, NotificationBody body);
        void PostMessage(NotificationAddress sender, NotificationAddress[] to, NotificationAddress[] cc, string subject, string body, bool isBodyHtml, NotificationAttachment[] attachments);
    }
}
