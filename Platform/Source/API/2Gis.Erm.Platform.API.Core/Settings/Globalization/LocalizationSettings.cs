using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DoubleGis.Erm.Platform.API.Core.Settings.Globalization
{
    public static class LocalizationSettings
    {
        public static readonly IReadOnlyDictionary<CultureInfo, CultureInfo> SupportedCultures2NeutralFallbackToMap =
            new Dictionary<CultureInfo, CultureInfo>
                {
                    { new CultureInfo("ru-RU"), new CultureInfo("ru") },
                    { new CultureInfo("en-US"), new CultureInfo("en") },
                    { new CultureInfo("el-GR"), new CultureInfo("el") },
                    { new CultureInfo("cs-CZ"), new CultureInfo("cs") },
                    { new CultureInfo("es-CL"), new CultureInfo("es") },
                    { new CultureInfo("ar-AE"), new CultureInfo("ar") },
                    { new CultureInfo("kk-KZ"), new CultureInfo("kk") },
                };

        internal static readonly CultureInfo[] FallbackOnlySupportedCultures = { new CultureInfo("el"), new CultureInfo("el-GR") };

        internal static CultureInfo[] SupportedCultures
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
    }
}
