using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace DoubleGis.Erm.Platform.WCF.Extensions
{
    public class SetMaxFaultSizeBehavior : IEndpointBehavior
    {
        private readonly int _maxFaultSize;

        public SetMaxFaultSizeBehavior(int maxFaultSize)
        {
            _maxFaultSize = maxFaultSize;
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MaxFaultSize = _maxFaultSize;
        }
    }
}