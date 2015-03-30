using NuClear.Settings.API;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings
{
    public interface IWebAppProcesingSettings : ISettings
    {
        int AuthExpirationTimeInMinutes { get; }
    }
}