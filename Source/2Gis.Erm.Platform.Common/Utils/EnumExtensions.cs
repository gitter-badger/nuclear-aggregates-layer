using System;
using System.Globalization;
using System.Resources;

namespace DoubleGis.Erm.Platform.Common.Utils
{
    public static class EnumExtensions
    {
        public static string ToStringLocalized(this Enum value, ResourceManager resourceManager, CultureInfo cultureInfo)
        {
            var type = value.GetType();
            var name = type.GetEnumName(value);

            if (resourceManager == null)
                return name;

            var localizedName = resourceManager.GetString(type.Name + name, cultureInfo);
            return localizedName ?? name;
        }
    }
}