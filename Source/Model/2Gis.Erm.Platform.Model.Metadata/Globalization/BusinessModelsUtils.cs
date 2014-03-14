using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DoubleGis.Erm.Platform.Model.Metadata.Globalization
{
    public static class BusinessModelsUtils
    {
        public static IEnumerable<BusinessModel> AdaptedForBusinessModels(this Type hostType)
        {
            var adaptedMap = new HashSet<BusinessModel>();

            foreach (var adaptedIndicator in hostType.GetInterfaces()
                                                     .Where(t => t.IsAdaptedType() && !BusinessModelsIndicators.Group.All.Contains(t))
                                                     .Distinct())
            {
                var spec = adaptedIndicator.GetCustomAttribute<GlobalizationSpecsAttribute>();
                if (spec == null)
                {
                    continue;
                }

                adaptedMap.Add(spec.BusinessModel);
            }

            return adaptedMap;
        }

        public static BusinessModel AsBusinessModel(this Type adaptedIndicator)
        {
            var spec = adaptedIndicator.GetCustomAttribute<GlobalizationSpecsAttribute>();
            if (spec == null)
            {
                throw new InvalidOperationException("Can't extract required specs for adapted indicator " + adaptedIndicator.FullName);
            }

            return spec.BusinessModel;
        }
    }
}