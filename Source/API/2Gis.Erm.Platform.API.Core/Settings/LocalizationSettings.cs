using System.Globalization;

namespace DoubleGis.Erm.Platform.API.Core.Settings
{
    public static class LocalizationSettings
    {
        public static readonly CultureInfo[] SupportedCultures =
            {
                new CultureInfo("ru-RU"), 
                new CultureInfo("en-US"), 
                new CultureInfo("el-GR"), 
                new CultureInfo("cs-CZ")
            };

        public static readonly CultureInfo DefaultCulture = new CultureInfo("ru-RU");
        public static readonly CultureInfo ApplicationCulture = DefaultCulture;
    }
}
