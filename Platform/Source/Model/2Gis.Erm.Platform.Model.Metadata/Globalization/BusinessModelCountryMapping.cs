using System;
using System.Collections.Generic;
using System.Linq;

namespace DoubleGis.Erm.Platform.Model.Metadata.Globalization
{
    public static class BusinessModelCountryMapping
    {
        // BusinessModel <-> [ISOCode, ..., ISOCode]
        private static readonly Lazy<IDictionary<BusinessModel, IEnumerable<int>>> Mapping =
            new Lazy<IDictionary<BusinessModel, IEnumerable<int>>>(() =>
                                                                   new Dictionary<BusinessModel, IEnumerable<int>>
                                                                       {
                                                                           { BusinessModel.Russia, new[] { 643 } },
                                                                           { BusinessModel.Cyprus, new[] { 196 } },
                                                                           { BusinessModel.Czech, new[] { 203 } },
                                                                           { BusinessModel.Chile, new[] { 152 } },
                                                                           { BusinessModel.Ukraine, new[] { 804 } },
                                                                           { BusinessModel.Emirates, new[] { 784 } },
                                                                           { BusinessModel.Kazakhstan, new[] { 398 } }
                                                                       });

        public static IEnumerable<int> GetCountryCodes(BusinessModel businessModel)
        {
            IEnumerable<int> countryCodes;
            return Mapping.Value.TryGetValue(businessModel, out countryCodes) ? countryCodes : Enumerable.Empty<int>();
        }
    }
}