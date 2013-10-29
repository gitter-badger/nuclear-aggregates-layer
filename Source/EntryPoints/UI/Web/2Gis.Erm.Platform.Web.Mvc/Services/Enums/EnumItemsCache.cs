using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Common.Collections;

namespace DoubleGis.Erm.Platform.Web.Mvc.Services.Enums
{
    public sealed class EnumItemsCache : Registry<Type, IEnumerable<EnumItem>>, IEnumItemsCache
    {
        private readonly DefaultEnumAdaptationService _defaultEnumConverter = new DefaultEnumAdaptationService();
        private readonly Dictionary<Type, IEnumAdaptationService> _adaptationEnumServices;

        public EnumItemsCache(Dictionary<Type, IEnumAdaptationService> adaptationEnumServices)
        {
            _adaptationEnumServices = adaptationEnumServices;
        }

        protected override IEnumerable<EnumItem> CreateValue(Type key)
        {
            IEnumAdaptationService enumAdaptationService;
            if (_adaptationEnumServices.TryGetValue(key, out enumAdaptationService))
            {
                return enumAdaptationService.GetEnumValues();
            }

            return _defaultEnumConverter.GetEnumValues(key);
        }
    }
}
