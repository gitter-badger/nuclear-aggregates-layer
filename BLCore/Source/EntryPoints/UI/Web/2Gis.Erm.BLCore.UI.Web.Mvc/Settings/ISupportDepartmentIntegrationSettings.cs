using NuClear.Settings.API;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings
{
    public interface ISupportDepartmentIntegrationSettings : ISettings
    {
        string SupportUrl { get; }
    }
}