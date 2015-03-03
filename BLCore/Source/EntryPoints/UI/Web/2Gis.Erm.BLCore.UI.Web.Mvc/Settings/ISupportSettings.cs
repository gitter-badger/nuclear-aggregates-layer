using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings
{
    public interface ISupportSettings : ISettings
    {
        string SupportUrl { get; }
    }
}