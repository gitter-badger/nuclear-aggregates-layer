using System.Globalization;

using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.Platform.API.Core.Settings.Globalization
{
    public interface ILocalizationSettings : ISettings
    {
        CultureInfo[] SupportedCultures { get; }

        CultureInfo DefaultCulture { get; }
        CultureInfo ApplicationCulture { get; }
    }
}