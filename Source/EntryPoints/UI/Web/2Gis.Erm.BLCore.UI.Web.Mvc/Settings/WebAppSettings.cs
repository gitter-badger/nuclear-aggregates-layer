using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings
{
    public sealed class WebAppSettings : CommonConfigFileAppSettings, IWebAppSettings
    {
        private readonly IntSetting _authExpirationTimeInMinutes = ConfigFileSetting.Int.Required("AuthExpirationTimeInMinutes");
        private readonly IntSetting _fileSizeLimit = ConfigFileSetting.Int.Required("FileSizeLimit");

        private readonly StringSetting _reportServer = ConfigFileSetting.String.Required("ReportServer");
        private readonly StringSetting _reportServerDateFormat = ConfigFileSetting.String.Required("ReportServerDateFormat");

        private readonly StringSetting _adConnectionDomainName = ConfigFileSetting.String.Required("ADConnectionDomainName");
        private readonly StringSetting _adConnectionLogin = ConfigFileSetting.String.Required("ADConnectionLogin");
        private readonly StringSetting _adConnectionPassword = ConfigFileSetting.String.Required("ADConnectionPassword");

        public int AuthExpirationTimeInMinutes
        {
            get { return _authExpirationTimeInMinutes.Value; }
        }

        public string ReportServer
        {
            get { return _reportServer.Value; }
        }

        public int FileSizeLimit
        {
            get { return _fileSizeLimit.Value; }
        }

        public string ReportServerDateFormat
        {
            get { return _reportServerDateFormat.Value; }
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