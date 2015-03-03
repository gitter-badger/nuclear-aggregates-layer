﻿using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.Settings;
using DoubleGis.Erm.BLCore.API.Common.Crosscutting.AD;
using DoubleGis.Erm.BLCore.API.Common.Settings;
using DoubleGis.Erm.BLCore.API.MoDi.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Deals.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLCore.API.OrderValidation.Remote.Settings;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging.Transports.ServiceBusForWindowsServer;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.Caching;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.UI.Web.Mvc.Settings
{
    /// <summary>
    /// Требования/соглашения см. в объявлении ISettingsContainer
    /// </summary>
    public sealed class WebAppSettings : 
        SettingsContainerBase,
        IReportsSettings,
        IWebAppProcesingSettings,
        ISupportSettings
    {
        private readonly IntSetting _authExpirationTimeInMinutes = ConfigFileSetting.Int.Required("AuthExpirationTimeInMinutes");
        private readonly StringSetting _reportServer = ConfigFileSetting.String.Required("ReportServer");
        private readonly StringSetting _supportUrl = ConfigFileSetting.String.Required("SupportUrl");

        public WebAppSettings(IEnumerable<Type> supportedBusinessModelIndicators)
        {
            Aspects
                .UseUsuallyRequiredFor(supportedBusinessModelIndicators)
                .Use<GetUserInfoFromAdSettingsAspect>()
                .Use<DebtProcessingSettingsAspect>()
                .Use<WarmClientProcessingSettingsAspect>()
                .Use<NotificationsSettingsAspect>()
                .Use<CachingSettingsAspect>()
                .Use<ValidateFileSettingsAspect>()
                .Use<OperationLoggingSettingsAspect>()
                .IfRequiredUseOperationLogging2ServiceBus()
                .Use(RequiredServices
                        .Is<APIIntrospectionServiceSettingsAspect>()
                        .Is<APIOrderValidationServiceSettingsAspect>()
                        .Is<APIIdentityServiceSettingsAspect>()
                        .Is<APIOperationsServiceSettingsAspect>()
                        .Is<APISpecialOperationsServiceSettingsAspect>()
                        .Is<APIMoDiServiceSettingsAspect>()
                        .Is<APIWebClientServiceSettingsAspect>());
        }

        int IWebAppProcesingSettings.AuthExpirationTimeInMinutes
        {
            get { return _authExpirationTimeInMinutes.Value; }
        }

        string IReportsSettings.ReportServer
        {
            get { return _reportServer.Value; }
        }

        string ISupportSettings.SupportUrl
        {
            get { return _supportUrl.Value; }
        }
    }
}