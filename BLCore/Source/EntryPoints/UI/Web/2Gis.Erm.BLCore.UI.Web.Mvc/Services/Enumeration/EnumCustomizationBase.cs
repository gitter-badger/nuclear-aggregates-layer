using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.UI.Web.Mvc.Services.Enums;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Enumeration
{
    public abstract class EnumCustomizationBase<T> : IEnumAdaptationService<T> where T : struct, IConvertible
    {
        private readonly DefaultEnumAdaptationService _defaultEnumAdaptationService = new DefaultEnumAdaptationService();

        public IEnumerable<EnumItem> GetEnumValues()
        {
            var defaultValues = _defaultEnumAdaptationService.GetEnumValues(typeof(T));

            // последовательность Enum из GetRequiredEnumValues() сохраняется
            return GetRequiredEnumValues()
                .Select(x => defaultValues.FirstOrDefault(v => v.EnumItemName == Enum.GetName(typeof(T), x)))
                .Where(x => x != null);
        }

        protected abstract IEnumerable<T> GetRequiredEnumValues();
    }
}