using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DoubleGis.Erm.Platform.Common.Utils.Resources
{
    public class ResourceGroupManager : IResourceGroupManager
    {
        private readonly IEnumerable<Action<CultureInfo>> _cultureSetterActions;

        public ResourceGroupManager(IEnumerable<Type> resourceTypes)
        {
            _cultureSetterActions = resourceTypes.Select(CreateCultureSetter).ToArray();
        }

        public void SetCulture(CultureInfo cultureInfo)
        {
            foreach (var cultureSetterAction in _cultureSetterActions)
            {
                cultureSetterAction(cultureInfo);
            }
        }

        private static Action<CultureInfo> CreateCultureSetter(Type resourceType)
        {
            return resourceType.CreateStaticPropertySetter<CultureInfo>("Culture");
        }
    }
}
