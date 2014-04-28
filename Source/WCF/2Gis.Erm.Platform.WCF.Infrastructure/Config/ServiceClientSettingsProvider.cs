using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.Config
{
    public sealed class ServiceClientSettingsProvider : IServiceClientSettingsProvider
    {
        private readonly IDictionary<Tuple<Type, Type>, EndpointParams> _contractToEndpointParamsMap = new Dictionary<Tuple<Type, Type>, EndpointParams>();

        public ServiceEndpoint GetEndpoint(Type contractType, Type bindingType)
        {
            EndpointParams endpointParams;
            if (!_contractToEndpointParamsMap.TryGetValue(Tuple.Create(contractType, bindingType), out endpointParams))
            {
                throw new InvalidOperationException(string.Format("Can't find endpoint for the contract {0}", contractType.FullName));
            }

            return endpointParams.ToServiceEndpoint();
        }

        public ServiceClientSettingsProvider AddEndpoint<TContract>(Binding binding,
                                                                    Uri baseUrl,
                                                                    string operationApiUrl,
                                                                    params IEndpointBehavior[] endpointBehaviors)
        {
            if (!Attribute.IsDefined(typeof(TContract), typeof(ServiceContractAttribute)))
            {
                throw new InvalidOperationException(string.Format("Can't add binding mapping since {0} is not an operation contract", typeof(TContract).FullName));
            }

            var endpoint = new EndpointParams(
                ContractDescription.GetContract(typeof(TContract)),
                binding,
                new EndpointAddress(new Uri(baseUrl, new Uri(operationApiUrl, UriKind.Relative))),
                endpointBehaviors);

            _contractToEndpointParamsMap[Tuple.Create(typeof(TContract), binding.GetType())] = endpoint;

            return this;
        }

        private class EndpointParams
        {
            private readonly EndpointAddress _address;
            private readonly IEndpointBehavior[] _behaviors;
            private readonly Binding _binding;
            private readonly ContractDescription _contractDescription;

            public EndpointParams(ContractDescription contractDescription, Binding binding, EndpointAddress address, params IEndpointBehavior[] behaviors)
            {
                _contractDescription = contractDescription;
                _binding = binding;
                _address = address;
                _behaviors = behaviors;
            }

            public ServiceEndpoint ToServiceEndpoint()
            {
                var endpoint = new ServiceEndpoint(_contractDescription, _binding, _address);
                foreach (var behavior in _behaviors)
                {
                    endpoint.Behaviors.Add(behavior);
                }

                return endpoint;
            }
        }
    }
}