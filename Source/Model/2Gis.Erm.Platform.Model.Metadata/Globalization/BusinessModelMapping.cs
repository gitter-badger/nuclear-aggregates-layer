using System;
using System.Collections.Generic;
using System.Linq;

namespace DoubleGis.Erm.Platform.Model.Metadata.Globalization
{
    public static class BusinessModelMapping
    {
        private static readonly Dictionary<BusinessModel, Type> Mappings =
            new Dictionary<BusinessModel, Type>
                {
                    { BusinessModel.Russia, typeof(IRussiaAdapted) },
                    { BusinessModel.Cyprus, typeof(ICyprusAdapted) },
                    { BusinessModel.Czech, typeof(ICzechAdapted) },
                    { BusinessModel.Chile, typeof(IChileAdapted) }
                };

        public static BusinessModel AsBusinessModel(this Type adapted)
        {
            if (!typeof(IAdapted).IsAssignableFrom(adapted))
            {
                throw new InvalidOperationException(string.Format("IAdapted interface must be assignable from type {0}", adapted.Name));
            }

            // Можно подумать об использовании BiDictionary
            // http://stackoverflow.com/questions/268321/bidirectional-1-to-1-dictionary-in-c-sharp
            var mapping = Mappings.SingleOrDefault(x => x.Value == adapted);
            if (mapping.Equals(default(KeyValuePair<BusinessModel, Type>)))
            {
                throw new NotSupportedException("Business model mapping not supported");
            }

            return mapping.Key;
        }

        public static Type AsAdapted(this BusinessModel businessModel)
        {
            var mapping = Mappings[businessModel];
            if (mapping == null)
            {
                throw new NotSupportedException("Business model mapping not supported");
            }

            return mapping;
        }
    }
}