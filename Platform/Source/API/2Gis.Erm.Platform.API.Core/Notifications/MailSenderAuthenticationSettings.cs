using System;

namespace DoubleGis.Erm.Platform.API.Core.Notifications
{
    public class MailSenderAuthenticationSettings
    {
        private readonly MailSenderAuthenticationType _authenticationType;

        public MailSenderAuthenticationSettings(MailSenderAuthenticationType authenticationType)
        {
            _authenticationType = authenticationType;
        }   

        public MailSenderAuthenticationType AuthenticationType
        {
            get { return _authenticationType; }
        }

        public String UserName { get; set; }
        public String UserPass { get; set; }
    }
}
