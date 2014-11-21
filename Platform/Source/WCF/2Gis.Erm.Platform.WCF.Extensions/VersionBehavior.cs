using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Xml.Linq;

namespace DoubleGis.Erm.Platform.WCF.Extensions
{
    public sealed class VersionBehavior : IServiceBehavior
    {
        private readonly IDictionary<Tuple<Uri, XName>, Version> _versions;
        private readonly string _adaptation;

        public VersionBehavior(string adaptation)
        {
            _versions = new Dictionary<Tuple<Uri, XName>, Version>();
            _adaptation = adaptation;
        }

        public void AddBindingParameters(ServiceDescription serviceDescription,
                                         ServiceHostBase serviceHostBase,
                                         Collection<ServiceEndpoint> endpoints,
                                         BindingParameterCollection bindingParameters)
        {
        }
        
        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (var endpoint in serviceDescription.Endpoints)
            {
                if (IsDiscoverySystemEndpoint(endpoint))
                {
                    endpoint.Behaviors.Add(new InjectVersionBehavior(_versions, _adaptation));
                }
                else
                {
                    endpoint.Behaviors.Add(new CollectVersionBehavior(_versions));
                }
            }
        }

        private static bool IsDiscoverySystemEndpoint(ServiceEndpoint endpoint)
        {
            return endpoint.IsSystemEndpoint &&
                IsDiscoveryContract(endpoint.Contract.Name, endpoint.Contract.Namespace);
        }

        private static bool IsDiscoveryContract(string contractName, string contractNamespace)
        {
            return IsDiscoveryContractName(contractName) && IsDiscoveryContractNamespace(contractNamespace);
        }

        private static bool IsDiscoveryContractName(string contractName)
        {
            return string.Equals(contractName, ProtocolStrings.ContractNames.DiscoveryAdhocContractName) ||
                string.Equals(contractName, ProtocolStrings.ContractNames.DiscoveryManagedContractName);
        }

        private static bool IsDiscoveryContractNamespace(string contractNamespace)
        {
            return string.Equals(contractNamespace, ProtocolStrings.VersionApril2005.Namespace) ||
                string.Equals(contractNamespace, ProtocolStrings.Version11.Namespace) ||
                string.Equals(contractNamespace, ProtocolStrings.VersionCd1.Namespace);
        }
    }
}
