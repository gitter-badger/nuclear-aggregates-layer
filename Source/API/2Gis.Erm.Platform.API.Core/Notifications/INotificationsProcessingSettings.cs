namespace DoubleGis.Erm.Platform.API.Core.Notifications
{
    public interface INotificationProcessingSettings
    {
        MailSenderAuthenticationSettings AuthenticationSettings { get; }
        NotificationAddress DefaultSender { get; }
        string SmtpServerHost { get; }
    }
}