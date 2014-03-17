using System.Configuration;

namespace DoubleGis.Erm.Platform.API.Core.Settings.APIServices.Config.Xml
{
    public sealed class ErmServiceDescriptionsConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("ermServices", IsDefaultCollection = true, IsRequired = true)]
        [ConfigurationCollection(typeof(KeyValueConfigurationCollection),
            AddItemName = "ermService",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public ErmServiceElementCollection ErmServices
        {
            get
            {
                return (ErmServiceElementCollection)base["ermServices"];
            }
        }
    }
}