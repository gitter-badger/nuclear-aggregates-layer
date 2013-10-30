using System;

using DoubleGis.Erm.BL.API.Releasing.Releases;
using DoubleGis.Erm.BL.WCF.Releasing.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.API.WCF.Releasing.Settings
{
    public sealed class ReleasingSettings : CommonConfigFileAppSettings, IReleasingSettings
    {
        private readonly BoolSetting _enableIntegration = ConfigFileSetting.Bool.Required("EnableIntegration");
        private readonly StringSetting _integrationApplicationName = ConfigFileSetting.String.Required("IntegrationApplicationName");
        private readonly EnumSetting<ExportIntegrationMode> _exportIntegrationMode =
            ConfigFileSetting.Enum.Required<ExportIntegrationMode>("ExportIntegrationMode");
        private readonly StringSetting _ftpExportSite = ConfigFileSetting.String.Required("FtpExportSite");
        private readonly StringSetting _ftpExportSitePassword = ConfigFileSetting.String.Required("FtpExportSitePassword");
        private readonly StringSetting _ftpExportSiteUsername = ConfigFileSetting.String.Required("FtpExportSiteUsername");

        public bool EnableIntegration
        {
            get { return _enableIntegration.Value; }
        }

        public string IntegrationApplicationName
        {
            get { return _integrationApplicationName.Value; }
        }

        public bool EnableRabbitMqQueue
        {
            get { throw new NotSupportedException(); }
        }

        public ExportIntegrationMode ExportIntegrationMode
        {
            get { return _exportIntegrationMode.Value; }
        }

        public string FtpExportSite
        {
            get { return _ftpExportSite.Value; }
        }

        public string FtpExportSiteUsername
        {
            get { return _ftpExportSiteUsername.Value; }
        }

        public string FtpExportSitePassword
        {
            get { return _ftpExportSitePassword.Value; }
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