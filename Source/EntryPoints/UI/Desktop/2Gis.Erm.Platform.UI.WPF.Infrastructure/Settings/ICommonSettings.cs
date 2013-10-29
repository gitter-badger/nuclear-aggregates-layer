using DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Settings;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Settings
{
    public interface ICommonSettings : ISettings
    {
        string ApiUrl { get; }
        bool EnableCaching { get; }
    }
}
