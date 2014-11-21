using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class DisplayNameLocalizedAttribute : Attribute
    {
        public DisplayNameLocalizedAttribute(string resourceKey)
        {
            ResourceManagerProvider = typeof(MetadataResources);
            ResourceKey = resourceKey;
        }

        public Type ResourceManagerProvider { get; private set; }
        public string ResourceKey { get; private set; }

        internal string GetLocalizedDisplayName(CultureInfo culture)
        {
            foreach (var staticProperty in ResourceManagerProvider.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (staticProperty.PropertyType != typeof(ResourceManager))
                {
                    continue;
                }

                var resourceManager = (ResourceManager)staticProperty.GetValue(null, null);
                return resourceManager.GetString(ResourceKey, culture);
            }

            return ResourceKey; // Fallback with the key name
        }
    }
}