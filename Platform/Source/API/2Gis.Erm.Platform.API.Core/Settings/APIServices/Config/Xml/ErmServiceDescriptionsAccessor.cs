using System;
using System.Configuration;
using System.Linq;

namespace DoubleGis.Erm.Platform.API.Core.Settings.APIServices.Config.Xml
{
    public static class ErmServiceDescriptionsAccessor
    {
        private const string ErmServicesSettingsSectionName = "ermServicesSettings";

        public static ErmServiceDescription[] GetErmServiceDescriptions()
        {
            return GetErmServiceDescriptions(ConfigurationManager.GetSection);
        }

        public static ErmServiceDescription[] GetErmServiceDescriptions(Configuration configuration)
        {
            return GetErmServiceDescriptions(configuration.GetSection);
        }

        private static ErmServiceDescription[] GetErmServiceDescriptions(
            Func<string, object> ermServicesSettingsSectionLoader)
        {
            var configSection = (ErmServiceDescriptionsConfigurationSection)ermServicesSettingsSectionLoader(ErmServicesSettingsSectionName);
            return configSection != null
                           ? configSection.ErmServices
                                     .Cast<ErmServiceElement>()
                                     .Select(x => new ErmServiceDescription(x.ServiceName, x.RestUrl, x.BaseUrl, x.SoapEndpointName))
                                     .ToArray()
                           : new ErmServiceDescription[0];
        }
    }
}