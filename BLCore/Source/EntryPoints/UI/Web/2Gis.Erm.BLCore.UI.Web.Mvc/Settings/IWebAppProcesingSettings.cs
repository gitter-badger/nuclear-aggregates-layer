using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings
{
    public interface IWebAppProcesingSettings : ISettings
    {
        int AuthExpirationTimeInMinutes { get; }
    }
}