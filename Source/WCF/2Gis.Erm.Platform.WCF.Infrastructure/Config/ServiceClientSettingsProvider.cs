using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.Config
{
    public class ServiceClientSettingsProvider : IServiceClientSettingsProvider
    {
        private readonly IDictionary<Tuple<Type, Type>, ServiceEndpoint> _contractToEndpointMap = new Dictionary<Tuple<Type, Type>, ServiceEndpoint>();

        public ServiceClientSettingsProvider AddEndpoint<TContract>(Binding binding, Uri baseUrl, string operationApiUrl)
        {
            if (!Attribute.IsDefined(typeof(TContract), typeof(ServiceContractAttribute)))
            {
                throw new InvalidOperationException(string.Format("Can't add binding mapping since {0} is not an operation contract", typeof(TContract).FullName));
            }

            _contractToEndpointMap[Tuple.Create(typeof(TContract), binding.GetType())] = new ServiceEndpoint(ContractDescription.GetContract(typeof(TContract)),
                                                                                                             binding,
                                                                                                             new EndpointAddress(new Uri(baseUrl,
                                                                                                                                         new Uri(operationApiUrl, UriKind.Relative))));

            return this;
        }

        public ServiceEndpoint GetEndpoint(Type contractType, Type bindingType)
        {
            ServiceEndpoint endpoint;
            if (!_contractToEndpointMap.TryGetValue(Tuple.Create(contractType, bindingType), out endpoint))
            {
                throw new InvalidOperationException(string.Format("Can't find endpoint for the contract {0}", contractType.FullName));
            }

            return endpoint;
        }
    }
}