using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Discovery;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.Config
{
    public class DiscoveryEndpointContainer : IDiscoveryEndpointContainer
    {
        private readonly List<DiscoveryEndpoint> _endpoints = new List<DiscoveryEndpoint>();

        public IEnumerable<DiscoveryEndpoint> Endpoints
        {
            get { return _endpoints; }
        }

        public DiscoveryEndpointContainer AddEndpoint(string name, Binding binding, Uri baseUrl, string operationApiUrl)
        {
            _endpoints.Add(new DiscoveryEndpoint(binding, new EndpointAddress(new Uri(baseUrl, new Uri(operationApiUrl, UriKind.Relative))))
                {
                    Name = name
                });

            return this;
        }
    }
}