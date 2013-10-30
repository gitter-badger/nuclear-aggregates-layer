using System;

using DoubleGis.Erm.BL.TaskService.Settings;
using DoubleGis.Erm.Platform.API.Core.Notifications;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.TaskService.Settings
{
    public class ErmServiceAppSettings : CommonConfigFileAppSettings, IErmServiceAppSettings
    {
        private const int LogSizeInDaysDefault = 60;
        private const string MailSenderUserNameDefault = "TEST";
        private const string MailSenderUserPassDefault = "TEST";
        private const string MailSenderEmailAddressDefault = "noreply.ermnotification@2gis.ru";
        private const string MailSenderEmailDisplayNameDefault = "ERM notification system";

        private readonly BoolSetting _enableIntegration = ConfigFileSetting.Bool.Required("EnableIntegration");
        private readonly StringSetting _integrationApplicationName = ConfigFileSetting.String.Required("IntegrationApplicationName");
        private readonly BoolSetting _enableRabbitMqQueue = ConfigFileSetting.Bool.Required("EnableRabbitMqQueue");
        private readonly StringSetting _regionalTerritoryLocaleSpecificWord = ConfigFileSetting.String.Required("RegionalTerritoryLocaleSpecificWord");

        private readonly IntSetting _maxWorkingThreads = ConfigFileSetting.Int.Required("MaxWorkingThreads");
        private readonly IntSetting _logSizeInDays = ConfigFileSetting.Int.Optional("LogSizeInDays", LogSizeInDaysDefault);
        
        private readonly EnumSetting<MailSenderAuthenticationType> _mailSenderAuthType =
            ConfigFileSetting.Enum.Required<MailSenderAuthenticationType>("MailSenderAuthType");
        private readonly StringSetting _mailSenderUserName = ConfigFileSetting.String.Optional("MailSenderUserName", MailSenderUserNameDefault);
        private readonly StringSetting _mailSenderUserPass = ConfigFileSetting.String.Optional("MailSenderUserPass", MailSenderUserPassDefault);
        private readonly StringSetting _mailSenderEmailAddress = ConfigFileSetting.String.Optional("MailSenderEmailAddress", MailSenderEmailAddressDefault);
        private readonly StringSetting _mailSenderEmailDisplayName = ConfigFileSetting.String.Optional("MailSenderEmailDisplayName", MailSenderEmailDisplayNameDefault);
        private readonly StringSetting _smtpServerHost = ConfigFileSetting.String.Required("SmtpServerHost");

        private readonly StringSetting _adConnectionDomainName = ConfigFileSetting.String.Required("ADConnectionDomainName");
        private readonly StringSetting _adConnectionLogin = ConfigFileSetting.String.Required("ADConnectionLogin");
        private readonly StringSetting _adConnectionPassword = ConfigFileSetting.String.Required("ADConnectionPassword");

        private readonly StringSetting _webApplicationRoot = ConfigFileSetting.String.Required("WebApplicationRoot");

        public Uri WebApplicationRoot
        {
            get
            {
                return new Uri(_webApplicationRoot.Value);
            }
        }

        public bool EnableIntegration
        {
            get
            {
                return _enableIntegration.Value;
            }
        }

        public string IntegrationApplicationName
        {
            get
            {
                return _integrationApplicationName.Value;
            }
        }

        public bool EnableRabbitMqQueue
        {
            get
            {
                return _enableRabbitMqQueue.Value;
            }
        }

        public int MaxWorkingThreads
        {
            get
            {
                return _maxWorkingThreads.Value;
            }
        }

        public int LogSizeInDays
        {
            get
            {
                return _logSizeInDays.Value;
            }
        }

        public MailSenderAuthenticationSettings AuthenticationSettings
        {
            get
            {
                var authenticationSettings = new MailSenderAuthenticationSettings(_mailSenderAuthType.Value);
                switch (authenticationSettings.AuthenticationType)
                {
                    case MailSenderAuthenticationType.None:
                    {
                        throw new ApplicationException("Notification setting is invalid. Specified authentication type is unsupported");
                    }

                    case MailSenderAuthenticationType.ClearText:
                    {
                        if (!_mailSenderUserName.IsContainedInAppSetting || !_mailSenderUserPass.IsContainedInAppSetting)
                        {
                            throw new ApplicationException("Notification setting is invalid. Specified authentication type: " + 
                                                           authenticationSettings.AuthenticationType +
                                                           ". So settings: " + _mailSenderUserName.SettingName + " and " + _mailSenderUserPass.SettingName +
                                                           " must have correct values");
                        }

                        authenticationSettings.UserName = _mailSenderUserName.Value;
                        authenticationSettings.UserPass = _mailSenderUserPass.Value;

                        break;
                    }
                }

                return authenticationSettings;
            }
        }

        public NotificationAddress DefaultSender
        {
            get
            {
                return new NotificationAddress(_mailSenderEmailAddress.Value, _mailSenderEmailDisplayName.Value);
            }
        }

        public string SmtpServerHost
        {
            get
            {
                return _smtpServerHost.Value;
            }
        }

        public string ADConnectionDomainName
        {
            get
            {
                return _adConnectionDomainName.Value;
            }
        }

        public string ADConnectionLogin
        {
            get
            {
                return _adConnectionLogin.Value;
            }
        }

        public string ADConnectionPassword
        {
            get
            {
                return _adConnectionPassword.Value;
            }
        }
        
        public string RegionalTerritoryLocaleSpecificWord
        {
            get { return _regionalTerritoryLocaleSpecificWord.Value; }
        }

        public IMsCrmSettings MsCrmSettings
        {
            get { return MsCRMSettings; }
        }

        public APIServicesSettingsAspect ServicesSettings
        {
            get { return APIServicesSettings; }
        }
    }
}