using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Settings
{
    public interface ICommonSettings : ISettings
    {
        bool EnableCaching { get; }
    }
}
