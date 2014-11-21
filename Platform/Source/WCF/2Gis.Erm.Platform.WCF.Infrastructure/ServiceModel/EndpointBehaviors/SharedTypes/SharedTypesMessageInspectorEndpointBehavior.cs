using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.EndpointBehaviors.SharedTypes
{
    public class SharedTypesMessageInspectorEndpointBehavior : IEndpointBehavior
    {
        private readonly IDispatchMessageInspector _messageInspector;

        public SharedTypesMessageInspectorEndpointBehavior(IDispatchMessageInspector messageInspector)
        {
            _messageInspector = messageInspector;
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(_messageInspector);
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }
    }
}