using System.Configuration;

namespace DoubleGis.Erm.BLCore.Releasing.Release.Configuration
{
    public sealed class ExportIntegrationSettingsSection : ConfigurationSection
    {
        [ConfigurationProperty("organizationUnits", IsDefaultCollection = true, IsRequired = true)]
        [ConfigurationCollection(typeof(KeyValueConfigurationCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public KeyValueConfigurationCollection OrganizationUnits
        {
            get { return (KeyValueConfigurationCollection) base["organizationUnits"]; }
        }
    }
}