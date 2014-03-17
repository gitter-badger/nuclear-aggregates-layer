using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.Platform.API.Core.Notifications
{
    public interface INotificationProcessingSettings : ISettings
    {
        MailSenderAuthenticationSettings AuthenticationSettings { get; }
        NotificationAddress DefaultSender { get; }
        string SmtpServerHost { get; }
    }
}