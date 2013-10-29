using System;
using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.API.Core.Globalization
{
    public static class BusinessModelMapping
    {
        private static readonly Dictionary<BusinessModel, Type> Mapping 
            = new Dictionary<BusinessModel, Type>(4);

        static BusinessModelMapping()
        {
            Mapping.Add(BusinessModel.Russia, typeof(IRussiaAdapted));
            Mapping.Add(BusinessModel.Cyprus, typeof(ICyprusAdapted));
            Mapping.Add(BusinessModel.Czech, typeof(ICzechAdapted));
        }

        public static Type GetMarkerInterfaceForAdaptation(BusinessModel adaptationToken)
        {
            if (adaptationToken == BusinessModel.NotSetted)
            {
                throw new ArgumentException("BusinessLogicAdaptation is set wrong");
            }

            return Mapping[adaptationToken];
        }
    }
}
