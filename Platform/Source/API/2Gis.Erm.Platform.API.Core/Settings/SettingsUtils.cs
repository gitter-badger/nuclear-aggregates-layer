using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Core.Settings.Environments;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;

using NuClear.Settings.API;

namespace DoubleGis.Erm.Platform.API.Core.Settings
{
    public static class SettingsUtils
    {
        public static ICollection<ISettingsAspect> UseUsuallyRequiredFor(
            this ICollection<ISettingsAspect> aspects, 
            IEnumerable<Type> supportedBusinessModelIndicators)
        {
            return aspects.Use(UsuallyRequiredFor(supportedBusinessModelIndicators));
        }

        private static IEnumerable<ISettingsAspect> UsuallyRequiredFor(IEnumerable<Type> supportedBusinessModelIndicators)
        {
            var connectionStrings = new ConnectionStringsSettingsAspect();

            return new ISettingsAspect[]
                {
                    connectionStrings,
                    new GlobalizationAspect(supportedBusinessModelIndicators),
                    new EnvironmentsAspect(),
                    new MsCRMSettingsAspect(connectionStrings)
                };
        }
    }
}