using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model;

namespace DoubleGis.Erm.Platform.Web.Mvc.Services.Enums
{
    public sealed class DefaultEnumAdaptationService
    {
        public IEnumerable<EnumItem> GetEnumValues(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new InvalidOperationException("Not a enum : " + enumType.Name);
            }

            var enumNames = enumType.GetEnumNames();

            // Для правильно обработки enum`ов, основанных не на Int32.
            var enumValues = enumType.GetEnumValues().Cast<object>().Select(Convert.ToInt64).ToArray();

            var enumItems = new EnumItem[enumValues.Length];
            var defaultValue = GetDefaultValue(enumType); 

            for (var i = 0; i < enumValues.Length; i++)
            {
                var enumValue = enumValues[i];

                enumItems[i] = new EnumItem
                {
                    // undefined value should be empty string because of client validation purposes
                    Value = (enumValue == defaultValue) ? string.Empty : enumNames[i],
                    EnumItemName = enumNames[i]
                };
            }

            return enumItems;
        }

        private static long GetDefaultValue(Type enumType)
        {
            if (!enumType.IsDefined(typeof(UndefinedEnumValueAttribute), false))
            {
                return 0;
            }

            return enumType
                .GetCustomAttributes(false)
                .OfType<UndefinedEnumValueAttribute>()
                .First()
                .Value;
        }
    }
}
