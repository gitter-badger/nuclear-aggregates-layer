using System;
using System.Configuration;

namespace DoubleGis.Erm.Platform.API.Core.Settings.APIServices.Config.Xml
{
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