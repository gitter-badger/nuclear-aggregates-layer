using DoubleGis.Erm.BLCore.API.Common.Crosscutting.AD;
using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings
{
    public interface IWebAppSettings : 
        IAppSettings,
        IGetUserInfoFromAdSettings,
        IMsCrmSettingsHost,
        IAPIServiceSettingsHost
    {
        int AuthExpirationTimeInMinutes { get; }
        string ReportServer { get; }
        int FileSizeLimit { get; }
        string ReportServerDateFormat { get; }
    }
}