using System;
using System.Collections.Generic;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Xml.Linq;

namespace DoubleGis.Erm.Platform.WCF.Extensions
{
    public sealed class CollectVersionBehavior : IEndpointBehavior
    {
        private readonly IDictionary<Tuple<Uri, XName>, Version> _versions;

        public CollectVersionBehavior(IDictionary<Tuple<Uri, XName>, Version> versions)
        {
            _versions = versions;
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            var endpointContractName = XName.Get(endpoint.Contract.Name, endpoint.Contract.Namespace);
            var endpointContractVersion = endpoint.Contract.ContractType.Assembly.GetName().Version;

            var key = Tuple.Create(endpoint.Address.Uri, endpointContractName);
            _versions.Add(key, endpointContractVersion);
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }
    }
}
