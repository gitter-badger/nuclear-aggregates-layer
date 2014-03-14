using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings
{
    public interface IReportsSettings : ISettings
    {
        string ReportServer { get; }
    }
}