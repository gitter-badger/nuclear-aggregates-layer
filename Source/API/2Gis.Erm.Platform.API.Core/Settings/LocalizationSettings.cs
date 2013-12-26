using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DoubleGis.Erm.Platform.API.Core.Settings
{
    public static class LocalizationSettings
    {
        public static readonly IReadOnlyDictionary<CultureInfo, CultureInfo> SupportedCultures2NeutralFallbackToMap =
            new Dictionary<CultureInfo, CultureInfo> {
                { new CultureInfo("ru-RU"), new CultureInfo("ru") },
                { new CultureInfo("en-US"), new CultureInfo("en") }, 
                { new CultureInfo("el-GR"), new CultureInfo("el") }, 
                { new CultureInfo("cs-CZ"), new CultureInfo("cs") }
            };

        public static readonly CultureInfo[] FallbackOnlySupportedCultures = { new CultureInfo("el"), new CultureInfo("el-GR") };

        public static CultureInfo[] SupportedCultures
        {
            get
            {
                return SupportedCultures2NeutralFallbackToMap.Keys.ToArray();
            }
        }

        public static CultureInfo[] NeutralFallbackCultures
        {
            get
            {
                return SupportedCultures2NeutralFallbackToMap.Values.ToArray();
            }
        }

        public static readonly CultureInfo DefaultCulture = new CultureInfo("ru-RU");
        public static readonly CultureInfo ApplicationCulture = DefaultCulture;
    }
}
