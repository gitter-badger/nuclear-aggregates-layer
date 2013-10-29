using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

using DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.Formatters;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.EndpointBehaviors.Json
{
    /// <summary>
    /// Нужен для подсовывания своего форматтера на серверную сторону.
    /// </summary>
    public sealed class JsonResponseFormatterEndpointBehavior : IEndpointBehavior
    {
        public void Validate(OperationDescription operationDescription)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            foreach (var op in endpointDispatcher.DispatchRuntime.Operations)
            {
                op.Formatter = new JsonSerializerServerFormatterWrapper(op.Formatter);
            }
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }
    }
}
