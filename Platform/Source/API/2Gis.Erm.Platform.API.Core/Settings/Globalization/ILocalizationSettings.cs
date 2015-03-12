using System.Globalization;

using NuClear.Settings.API;

namespace DoubleGis.Erm.Platform.API.Core.Settings.Globalization
{
    public interface ILocalizationSettings : ISettings
    {
        CultureInfo[] SupportedCultures { get; }

        CultureInfo DefaultCulture { get; }
        CultureInfo ApplicationCulture { get; }
    }
}