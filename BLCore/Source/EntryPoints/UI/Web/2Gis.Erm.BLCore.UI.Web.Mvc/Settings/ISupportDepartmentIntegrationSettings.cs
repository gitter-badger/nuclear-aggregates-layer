using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings
{
    public interface ISupportDepartmentIntegrationSettings : ISettings
    {
        string SupportUrl { get; }
    }
}