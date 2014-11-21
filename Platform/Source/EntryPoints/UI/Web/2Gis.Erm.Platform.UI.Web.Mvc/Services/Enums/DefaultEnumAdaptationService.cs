using System;
using System.Collections.Generic;
using System.Linq;

namespace DoubleGis.Erm.Platform.UI.Web.Mvc.Services.Enums
{
    public sealed class DefaultEnumAdaptationService
    {
        public IEnumerable<EnumItem> GetEnumValues(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new InvalidOperationException("Not a enum : " + enumType.Name);
            }

            return enumType.GetEnumValues().Cast<object>().Select(CreateEnumItem).ToArray();
        }

        private EnumItem CreateEnumItem(object enumValue)
        {
            return new EnumItem
                {
                    Value = EnumUIUtils.GetEnumName(enumValue, true),
                    EnumItemName = EnumUIUtils.GetEnumName(enumValue, false)
                };
        }
    }
}
