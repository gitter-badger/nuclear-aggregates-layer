using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;

namespace DoubleGis.Erm.Platform.Web.Mvc.Settings
{
    public interface IWebAppSettings : 
        IAppSettings,
        IMsCrmSettingsHost,
        IAPIServiceSettingsHost
    {
        int AuthExpirationTimeInMinutes { get; }
        string ReportServer { get; }
        int FileSizeLimit { get; }
        string ReportServerDateFormat { get; }
    }
}