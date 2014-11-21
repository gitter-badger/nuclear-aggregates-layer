using System;
using System.Reflection;

using DoubleGis.Erm.Platform.Model;

namespace DoubleGis.Erm.Platform.UI.Web.Mvc.Services.Enums
{
    public static class EnumUIUtils
    {
        public static long GetDefaultValue(Type enumType)
        {
            var undefinedEnumValueAttribute = enumType.GetCustomAttribute<UndefinedEnumValueAttribute>();
            return undefinedEnumValueAttribute != null ? undefinedEnumValueAttribute.Value : 0;
        }

        public static string GetEnumName(object enumValue, bool emptyStringForDefaultValue)
        {
            var numericValue = Convert.ToInt64(enumValue);
            var type = enumValue.GetType();

            return numericValue == GetDefaultValue(type) && emptyStringForDefaultValue ? string.Empty : type.GetEnumName(enumValue);
        }
    }
}