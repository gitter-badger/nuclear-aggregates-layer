using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Logging;
using DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.EndpointBehaviors.SharedTypes;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.ServiceBehaviors
{
    public sealed class ErmServiceBehavior : IServiceBehavior
    {
        private readonly IInstanceProviderFactory _instanceProviderFactory;
        private readonly IDispatchMessageInspectorFactory _messageInspectorFactory;
        private readonly IErrorHandlerFactory _errorHandlerFactory;
        private readonly ISharedTypesBehaviorFactory _sharedTypesBehaviorFactory;

        public ErmServiceBehavior(IInstanceProviderFactory instanceProviderFactory,
                                  IDispatchMessageInspectorFactory messageInspectorFactory,
                                  IErrorHandlerFactory errorHandlerFactory,
                                  ISharedTypesBehaviorFactory sharedTypesBehaviorFactory)
        {
            _instanceProviderFactory = instanceProviderFactory;
            _messageInspectorFactory = messageInspectorFactory;
            _errorHandlerFactory = errorHandlerFactory;
            _sharedTypesBehaviorFactory = sharedTypesBehaviorFactory;
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            var sharedTypesBehavior = _sharedTypesBehaviorFactory.Create();
            var instanceProvider = _instanceProviderFactory.Create(serviceDescription.ServiceType);
            var messageInspector = _messageInspectorFactory.Create();
            var errorHandler = _errorHandlerFactory.Create();

            foreach (var endpoint in serviceDescription.Endpoints)
            {

                if (endpoint.Binding is WSHttpBindingBase || endpoint.Binding is WSDualHttpBinding)
                {
                    endpoint.Behaviors.Add(sharedTypesBehavior);
                }
            }
            
            foreach (var channelDispatcher in serviceHostBase.ChannelDispatchers.OfType<ChannelDispatcher>())
            {
                foreach (var endpointDispatcher in channelDispatcher.Endpoints)
                {
                    if (ServiceNamespaces.IsErmService(endpointDispatcher.ContractNamespace))
                    {
                        endpointDispatcher.DispatchRuntime.InstanceProvider = instanceProvider;
                    }

                    endpointDispatcher.DispatchRuntime.MessageInspectors.Add(messageInspector);
                }

                channelDispatcher.ErrorHandlers.Add(errorHandler);
            }
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        public void AddBindingParameters(ServiceDescription serviceDescription,
                                         ServiceHostBase serviceHostBase,
                                         Collection<ServiceEndpoint> endpoints,
                                         BindingParameterCollection bindingParameters)
        {
        }
    }
}
