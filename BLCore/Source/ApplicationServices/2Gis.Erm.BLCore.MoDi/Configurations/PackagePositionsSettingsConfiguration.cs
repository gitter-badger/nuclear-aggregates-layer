using System.Configuration;
using System.Globalization;

using DoubleGis.Erm.BLCore.API.MoDi.Enums;

namespace DoubleGis.Erm.BLCore.MoDi.Configurations
{
    public static class PackagePositionsSettings
    {
        public static PackagePositionsSettingsConfiguration Instance
        {
            get
            {
                return (PackagePositionsSettingsConfiguration)ConfigurationManager.GetSection("packagePositionSettings");
            }
        }
    }

    public sealed class PackagePositionsSettingsConfiguration: ConfigurationSection
    {
        [ConfigurationProperty("packagePositions")]
        public PackagePositions PackagePositions
        {
            get { return (PackagePositions)this["packagePositions"]; }
        }
    }

    [ConfigurationCollection(typeof(PackagePositions), AddItemName = "packagePosition")]
    public sealed class PackagePositions : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new PackagePosition();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var packagePosition = ((PackagePosition)element);
            return packagePosition.Id + packagePosition.PriceId.ToString(CultureInfo.InvariantCulture);
        }
    }

    public sealed class PackagePosition : ConfigurationElement
    {
        [ConfigurationProperty("id", IsKey = false, IsRequired = true)]
        public long Id
        {
            get { return (long)this["id"]; }
            set { this["id"] = value; }
        }

        [ConfigurationProperty("priceId", IsKey = false, IsRequired = true)]
        public long PriceId
        {
            get { return (long)this["priceId"]; }
            set { this["priceId"] = value; }
        }

        [ConfigurationProperty("childPositions")]
        public ChildPositions ChildPositions
        {
            get { return (ChildPositions)this["childPositions"]; }
        }
    }

    [ConfigurationCollection(typeof(ConfigPosition), AddItemName = "position")]
    public sealed class ChildPositions : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ConfigPosition();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var configPosition = ((ConfigPosition) element);
            return configPosition.Id + configPosition.Platform.ToString();
        }
    }

    public sealed class ConfigPosition : ConfigurationElement
    {
        [ConfigurationProperty("id", IsKey = false, IsRequired = true)]
        public long Id
        {
            get { return (long)this["id"]; }
            set { this["id"] = value; }
        }

        [ConfigurationProperty("platform", IsKey = false, IsRequired = true)]
        public PlatformsExtended Platform
        {
            get { return (PlatformsExtended)this["platform"]; }
            set { this["platform"] = value; }
        }

        [ConfigurationProperty("cost", IsKey = false, IsRequired = true)]
        public decimal Cost
        {
            get { return (decimal)this["cost"]; }
            set { this["cost"] = value; }
        }
    }
}