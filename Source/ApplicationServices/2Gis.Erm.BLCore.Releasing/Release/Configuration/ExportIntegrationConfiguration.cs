using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace DoubleGis.Erm.BLCore.Releasing.Release.Configuration
{
    public static class ExportIntegrationConfiguration
    {
        private static ExportIntegrationSettingsSection _instance;

        public static IEnumerable<KeyValueConfigurationElement> OrganizationUnits
        {
            get 
            {
                if (_instance == null)
                {
                    _instance = (ExportIntegrationSettingsSection) ConfigurationManager.GetSection("exportIntegrationSettings");
                }
                return _instance.OrganizationUnits.Cast<KeyValueConfigurationElement>().ToArray();
            }
        }
    }
}