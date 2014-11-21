using System;

namespace DoubleGis.Erm.Platform.API.Core.Settings.APIServices.Config
{
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