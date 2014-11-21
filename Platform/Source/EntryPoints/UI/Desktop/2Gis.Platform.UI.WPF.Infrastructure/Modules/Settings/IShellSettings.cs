using System.Globalization;

namespace DoubleGis.Platform.UI.WPF.Infrastructure.Modules.Settings
{
    public interface IShellSettings
    {
        CultureInfo TargetCulture { get; }
    }
}
