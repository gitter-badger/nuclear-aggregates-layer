using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Xml.Linq;

using DoubleGis.Erm.Platform.API.Core.Metadata;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Config;

namespace DoubleGis.Erm.Platform.Core.Metadata
{
    public sealed class ServiceAvailabilityProvider : IServiceAvailabilityProvider
    {
        private static readonly TimeSpan DicoveryTimeout = new TimeSpan(0, 0, 0, 5);

        private readonly IClientCompatibilityProvider _compatibilityProvider;
        private readonly IDiscoveryEndpointContainer _discoveryEndpointContainer;

        public ServiceAvailabilityProvider(IClientCompatibilityProvider compatibilityProvider, IDiscoveryEndpointContainer discoveryEndpointContainer)
        {
            _compatibilityProvider = compatibilityProvider;
            _discoveryEndpointContainer = discoveryEndpointContainer;
        }

        public EndpointDescription[] GetCompatibleServices(Version clientVersion)
        {
            var availableServices = GetAvailableServices();
            return availableServices
                .Where(description => _compatibilityProvider.IsCompatible(clientVersion, description.Version, description.Name))
                .ToArray();
        }

        public EndpointDescription[] GetAvailableServices()
        {
            var declaredServices = _discoveryEndpointContainer.Endpoints;
            var availableServices = new List<EndpointDescription>();
            var criteria = new FindCriteria { Duration = DicoveryTimeout };
            var searchResult = ServiceSearch(declaredServices, criteria);

            foreach (var service in searchResult)
            {
                var findResult = service.Value;
                if (findResult == null || findResult.Endpoints.Count == 0)
                {
                    availableServices.Add(CreateUnavailableEndpointDescription(service.Key));
                }
                else
                {
                    foreach (var endpoint in findResult.Endpoints)
                    {
                        var endpointDescriptions = endpoint.ContractTypeNames.Count == 0
                                                       ? CreateAddressOnlyDescription(endpoint, service.Key)
                                                       : CreateEndpointDescriptions(endpoint, service.Key);
                        availableServices.AddRange(endpointDescriptions);
                    }
                }
            }

            return availableServices.ToArray();
        }

        private static IDictionary<string, FindResponse> ServiceSearch(IEnumerable<DiscoveryEndpoint> serviceNames, FindCriteria criteria)
        {
            return serviceNames.AsParallel().Select(serviceName =>
                {
                    Tuple<string, FindResponse> findResult;
                    var client = new DiscoveryClient(serviceName);
                    try
                    {
                        findResult = new Tuple<string, FindResponse>(serviceName.Name, client.Find(criteria));
                    }
                    catch (EndpointNotFoundException)
                    {
                        findResult = new Tuple<string, FindResponse>(serviceName.Name, null);
                    }
                    catch (TargetInvocationException)
                    {
                        findResult = new Tuple<string, FindResponse>(serviceName.Name, null);
                    }

                    client.Close();

                    return findResult;
                })
                .ToDictionary(tuple => tuple.Item1, tuple => tuple.Item2);
        }

        private static EndpointDescription CreateUnavailableEndpointDescription(string name)
        {
            return new EndpointDescription
                        {
                            Name = name,
                            Available = false
                        };
        }

        private static IEnumerable<EndpointDescription> CreateEndpointDescriptions(EndpointDiscoveryMetadata endpoint, string name)
        {
            return endpoint.ContractTypeNames.Select(qualifiedName => new EndpointDescription
                {
                    Name = name,
                    Address = endpoint.Address.Uri,
                    Version = ReadVersionExtension(endpoint.Extensions),
                    BusinessLogicAdaptation = ReadBusinessLogicAdaptationExtension(endpoint.Extensions),
                    ContractName = qualifiedName.Name,
                    ContractNamespace = qualifiedName.Namespace,
                    Available = true,
                });
        }

        private static IEnumerable<EndpointDescription> CreateAddressOnlyDescription(EndpointDiscoveryMetadata endpoint, string name)
        {
            return new[]
                {
                    new EndpointDescription
                        {
                            Name = name,
                            Address = endpoint.Address.Uri,
                            Version = ReadVersionExtension(endpoint.Extensions),
                            Available = false,
                        }
                };
        }

        private static string ReadBusinessLogicAdaptationExtension(IEnumerable<XElement> extensions)
        {
            var element = extensions.FirstOrDefault(e =>
                string.Equals(e.Name.LocalName, "BusinessLogicAdaptation", StringComparison.InvariantCultureIgnoreCase));

            return element != null ? element.Value : null;
        }

        private static Version ReadVersionExtension(IEnumerable<XElement> extensions)
        {
            var element = extensions.FirstOrDefault(e => 
                string.Equals(e.Name.LocalName, "ServiceVersion", StringComparison.InvariantCultureIgnoreCase));

            Version result;
            Version.TryParse(element != null ? element.Value : string.Empty, out result);
            return result;
        }
    }
}