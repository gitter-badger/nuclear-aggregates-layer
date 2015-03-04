using Nuclear.Settings.API;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Settings
{
    public interface ICommonSettings : ISettings
    {
        bool EnableCaching { get; }
    }
}
