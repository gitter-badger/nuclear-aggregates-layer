using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace DoubleGis.Erm.Platform.API.Core.Settings.APIServices
{
    public static class ErmServiceDescriptionsConfiguration
    {
        private static readonly Lazy<ErmServiceDescriptionsConfigurationSection> Instance =
            new Lazy<ErmServiceDescriptionsConfigurationSection>(
                () => (ErmServiceDescriptionsConfigurationSection)ConfigurationManager.GetSection("ermServicesSettings"));

        public static IEnumerable<ErmServiceDescription> ErmServices
        {
            get
            {
                var settings = Instance.Value;
                return settings != null
                           ? settings.ErmServices
                                     .Cast<ErmServiceElement>()
                                     .Select(x => new ErmServiceDescription(x.ServiceName, x.RestUrl, x.BaseUrl, x.SoapEndpointName))
                                     .ToArray()
                           : Enumerable.Empty<ErmServiceDescription>();
            }
        }

        public sealed class ErmServiceDescription
        {
            public ErmServiceDescription(string serviceName, Uri restUri, Uri baseUri, string soapEndpointName)
            {
                ServiceName = serviceName;
                RestUrl = restUri;
                BaseUrl = baseUri;
                SoapEndpointName = soapEndpointName;
            }

            public string ServiceName { get; private set; }
            public Uri RestUrl { get; private set; }
            public Uri BaseUrl { get; private set; }
            public string SoapEndpointName { get; private set; }
        }
    }

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

    public sealed class ErmServiceElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ErmServiceElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ErmServiceElement)element).ServiceName;
        }
    }

    public sealed class ErmServiceElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string ServiceName
        {
            get { return (string)this["name"]; }
        }

        [ConfigurationProperty("restUrl", IsRequired = true)]
        public Uri RestUrl
        {
            get { return new Uri(this["restUrl"].ToString()); }
        }

        [ConfigurationProperty("baseUrl", IsRequired = true)]
        public Uri BaseUrl
        {
            get { return new Uri(this["baseUrl"].ToString()); }
        }

        [ConfigurationProperty("soapEndpointName", IsRequired = false)]
        public string SoapEndpointName
        {
            get { return (string)this["soapEndpointName"]; }
        }
    }
}