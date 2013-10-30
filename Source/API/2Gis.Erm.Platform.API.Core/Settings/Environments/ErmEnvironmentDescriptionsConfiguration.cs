using System.Configuration;

using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;

namespace DoubleGis.Erm.Platform.API.Core.Settings.Environments
{
    public sealed class ErmEnvironmentsDescriptionsConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("ermEnvironments", IsDefaultCollection = true, IsRequired = true)]
        [ConfigurationCollection(typeof(KeyValueConfigurationCollection),
            AddItemName = "ermEnvironment",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public ErmEnvironmentElementCollection ErmEnvironments
        {
            get
            {
                return (ErmEnvironmentElementCollection)base["ermEnvironments"];
            }
        }
    }


    public class ErmEnvironmentElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ErmEnvironmentElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ErmEnvironmentElement)element).Name;
        }
    }

    public class ErmEnvironmentElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this["name"]; }
        }

        [ConfigurationProperty("connectionStrings")]
        public ConnectionStringSettingsCollection ConnectionStrings
        {
            get { return (ConnectionStringSettingsCollection)this["connectionStrings"]; }
        }

        [ConfigurationProperty("ermServices")]
        [ConfigurationCollection(typeof(KeyValueConfigurationCollection),
            AddItemName = "ermService",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public ErmServiceElementCollection ErmServices
        {
            get { return (ErmServiceElementCollection)this["ermServices"]; }
        }

        [ConfigurationProperty("entryPointOverrides")]
        [ConfigurationCollection(typeof(KeyValueConfigurationCollection),
            AddItemName = "entryPointOverride",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public EntryPointOverrideElementCollection EntryPointsOverrides
        {
            get { return (EntryPointOverrideElementCollection)this["entryPointOverrides"]; }
        }
    }


    public class EntryPointOverrideElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new EntryPointOverrideElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((EntryPointOverrideElement)element).EntryPointName;
        }
    }

    public class EntryPointOverrideElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string EntryPointName
        {
            get { return (string)this["name"]; }
        }

        [ConfigurationProperty("connectionStrings")]
        public ConnectionStringSettingsCollection ConnectionStrings
        {
            get { return (ConnectionStringSettingsCollection)this["connectionStrings"]; }
        }

        [ConfigurationProperty("ermServices")]
        [ConfigurationCollection(typeof(KeyValueConfigurationCollection),
            AddItemName = "ermService",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public ErmServiceElementCollection ErmServices
        {
            get { return (ErmServiceElementCollection)this["ermServices"]; }
        }
    }
}