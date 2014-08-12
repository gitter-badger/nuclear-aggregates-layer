﻿using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.Settings;
using DoubleGis.Erm.BLCore.API.Common.Crosscutting.AD;
using DoubleGis.Erm.BLCore.API.Common.Settings;
using DoubleGis.Erm.BLCore.API.MoDi.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.Platform.API.Core.Notifications;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing;
using DoubleGis.Erm.Platform.API.Core.PersistenceCleanup;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.Common.Settings;
using DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.ServiceBusForWindowsServer.Settings;
using DoubleGis.Erm.Platform.Core.Operations.Processing.Final.MsCRM;
using DoubleGis.Erm.Platform.Core.Operations.Processing.Primary.Transports.ServiceBusForWindowsServer.Settings;
using DoubleGis.Erm.Platform.TaskService.Settings;
using DoubleGis.Erm.Qds.Common.Settings;

namespace DoubleGis.Erm.TaskService.Settings
{
    /// <summary>
    /// Требования/соглашения см. в объявлении ISettingsContainer
    /// </summary>
    public sealed class TaskServiceAppSettings : 
        SettingsContainerBase,
        INotificationProcessingSettings,
        IIntegrationLocalizationSettings,
        IDBCleanupSettings,
        ITaskServiceProcesingSettings
    {
        private const int LogSizeInDaysDefault = 60;
        private const string MailSenderUserNameDefault = "TEST";
        private const string MailSenderUserPassDefault = "TEST";
        private const string MailSenderEmailAddressDefault = "noreply.ermnotification@2gis.ru";
        private const string MailSenderEmailDisplayNameDefault = "ERM notification system";

        private readonly IntSetting _maxWorkingThreads = ConfigFileSetting.Int.Required("MaxWorkingThreads");

        private readonly IntSetting _logSizeInDays = ConfigFileSetting.Int.Optional("LogSizeInDays", LogSizeInDaysDefault);
        
        private readonly EnumSetting<MailSenderAuthenticationType> _mailSenderAuthType =
            ConfigFileSetting.Enum.Required<MailSenderAuthenticationType>("MailSenderAuthType");
        private readonly StringSetting _mailSenderUserName = ConfigFileSetting.String.Optional("MailSenderUserName", MailSenderUserNameDefault);
        private readonly StringSetting _mailSenderUserPass = ConfigFileSetting.String.Optional("MailSenderUserPass", MailSenderUserPassDefault);
        private readonly StringSetting _mailSenderEmailAddress = ConfigFileSetting.String.Optional("MailSenderEmailAddress", MailSenderEmailAddressDefault);
        private readonly StringSetting _mailSenderEmailDisplayName = ConfigFileSetting.String.Optional("MailSenderEmailDisplayName", MailSenderEmailDisplayNameDefault);
        private readonly StringSetting _smtpServerHost = ConfigFileSetting.String.Required("SmtpServerHost");

        private readonly StringSetting _basicLanguage = ConfigFileSetting.String.Required("BasicLanguage");
        private readonly StringSetting _reserveLanguage = ConfigFileSetting.String.Required("ReserveLanguage");
        private readonly StringSetting _regionalTerritoryLocaleSpecificWord = ConfigFileSetting.String.Required("RegionalTerritoryLocaleSpecificWord");

        public TaskServiceAppSettings(IEnumerable<Type> supportedBusinessModelIndicators)
        {
            Aspects
               .UseUsuallyRequiredFor(supportedBusinessModelIndicators)
               .Use<GetUserInfoFromAdSettingsAspect>()
               .Use<DebtProcessingSettingsAspect>()
               .Use<IntegrationSettingsAspect>()
               .Use<NotificationsSettingsAspect>()
               .Use<CachingSettingsAspect>()
               .UseElasticClientNestSettingsAspect()
               .Use<OperationLoggingSettingsAspect>()
               .IfRequiredUseOperationLogging2ServiceBus()
               .Use<PerformedOperationsTransportSettingsAspect>()
               .IfRequiredUsePerformedOperationsFromServiceBusAspect()
               .Use<AsyncMsCRMReplicationSettingsAspect>()
               .Use(RequiredServices
                       .Is<APIIdentityServiceSettingsAspect>()
                       .Is<APIMoDiServiceSettingsAspect>());
        }

        string IIntegrationLocalizationSettings.BasicLanguage
        {
            get { return _basicLanguage.Value; }
        }

        string IIntegrationLocalizationSettings.ReserveLanguage
        {
            get { return _reserveLanguage.Value; }
        }

        string IIntegrationLocalizationSettings.RegionalTerritoryLocaleSpecificWord
        {
            get { return _regionalTerritoryLocaleSpecificWord.Value; }
        }

        int ITaskServiceProcesingSettings.MaxWorkingThreads
        {
            get
            {
                return _maxWorkingThreads.Value;
            }
        }

        int IDBCleanupSettings.LogSizeInDays
        {
            get
            {
                return _logSizeInDays.Value;
            }
        }

        MailSenderAuthenticationSettings INotificationProcessingSettings.AuthenticationSettings
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

        NotificationAddress INotificationProcessingSettings.DefaultSender
        {
            get
            {
                return new NotificationAddress(_mailSenderEmailAddress.Value, _mailSenderEmailDisplayName.Value);
            }
        }

        string INotificationProcessingSettings.SmtpServerHost
        {
            get
            {
                return _smtpServerHost.Value;
            }
        }
    }
}