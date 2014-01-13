using System.Configuration;

using DoubleGis.Erm.BLCore.API.MoDi.Enums;

namespace DoubleGis.Erm.BLCore.MoDi.Configurations
{
    public static class SaleSchemasSettings
    {
        public static SaleSchemasSettingsConfiguration Instance
        {
            get
            {
                return (SaleSchemasSettingsConfiguration)ConfigurationManager.GetSection("saleSchemasSettings");
            }
        }
    }

    public sealed class SaleSchemasSettingsConfiguration: ConfigurationSection
    {
        [ConfigurationProperty("saleSchemas")]
        public SaleSchemas SaleSchemas
        {
            get { return (SaleSchemas)this["saleSchemas"]; }
        }
    }

    [ConfigurationCollection(typeof(SaleSchema), AddItemName = "saleSchema")]
    public sealed class SaleSchemas : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new SaleSchema();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var saleSchema = ((SaleSchema)element);
            return saleSchema.Source.ToString() + saleSchema.Dest;
        }
    }

    public sealed class SaleSchema : ConfigurationElement
    {
        [ConfigurationProperty("source", IsKey = false, IsRequired = true)]
        public ESalesPointType Source
        {
            get { return (ESalesPointType)this["source"]; }
            set { this["source"] = value; }
        }

        [ConfigurationProperty("dest", IsKey = false, IsRequired = true)]
        public ESalesPointType Dest
        {
            get { return (ESalesPointType)this["dest"]; }
            set { this["dest"] = value; }
        }

        [ConfigurationProperty("platforms")]
        public Platforms Platforms
        {
            get { return (Platforms)this["platforms"]; }
        }
    }

    [ConfigurationCollection(typeof(Platform), AddItemName = "platform")]
    public sealed class Platforms : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new Platform();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var platform = ((Platform)element);
            return platform.Id.ToString() + platform.Direction;
        }
    }

    internal sealed class Platform : ConfigurationElement
    {
        [ConfigurationProperty("id", IsKey = false, IsRequired = true)]
        public PlatformsExtended Id
        {
            get { return (PlatformsExtended)this["id"]; }
            set { this["id"] = value; }
        }

        [ConfigurationProperty("direction", IsKey = false, IsRequired = true)]
        public ESalesPointType Direction
        {
            get { return (ESalesPointType)this["direction"]; }
            set { this["direction"] = value; }
        }
        [ConfigurationProperty("multiplier", IsKey = false, IsRequired = true)]
        public decimal Multiplier
        {
            get { return (decimal)this["multiplier"]; }
            set { this["multiplier"] = value; }
        }
    }
}