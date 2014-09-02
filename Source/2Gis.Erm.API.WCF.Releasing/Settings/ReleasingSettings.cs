using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.BLCore.API.OrderValidation.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Releasing.Releases;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging.Transports.ServiceBusForWindowsServer;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.Caching;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.API.WCF.Releasing.Settings
{
    /// <summary>
    /// Требования/соглашения см. в объявлении ISettingsContainer
    /// </summary>
    public sealed class ReleasingSettings : SettingsContainerBase, IFtpExportIntegrationModeSettings
    {
        private readonly EnumSetting<ExportIntegrationMode> _exportIntegrationMode =
            ConfigFileSetting.Enum.Required<ExportIntegrationMode>("ExportIntegrationMode");
        private readonly StringSetting _ftpExportSite = ConfigFileSetting.String.Required("FtpExportSite");
        private readonly StringSetting _ftpExportSitePassword = ConfigFileSetting.String.Required("FtpExportSitePassword");
        private readonly StringSetting _ftpExportSiteUsername = ConfigFileSetting.String.Required("FtpExportSiteUsername");

        public ReleasingSettings(IEnumerable<Type> supportedBusinessModelIndicators)
        {
            Aspects
               .UseUsuallyRequiredFor(supportedBusinessModelIndicators)
               .Use<IntegrationSettingsAspect>()
               .Use<CachingSettingsAspect>()
               .Use<OperationLoggingSettingsAspect>()
               .IfRequiredUseOperationLogging2ServiceBus()
               .Use(RequiredServices
                       .Is<APIOrderValidationServiceSettingsAspect>()
                       .Is<APIIdentityServiceSettingsAspect>());
        }

        ExportIntegrationMode IFtpExportIntegrationModeSettings.ExportIntegrationMode
        {
            get { return _exportIntegrationMode.Value; }
        }

        string IFtpExportIntegrationModeSettings.FtpExportSite
        {
            get { return _ftpExportSite.Value; }
        }

        string IFtpExportIntegrationModeSettings.FtpExportSiteUsername
        {
            get { return _ftpExportSiteUsername.Value; }
        }

        string IFtpExportIntegrationModeSettings.FtpExportSitePassword
        {
            get { return _ftpExportSitePassword.Value; }
        }
    }
}