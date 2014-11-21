using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.EndpointBehaviors.Jsonp
{
    // Original source and description is located here: http://msdn.microsoft.com/en-us/library/cc716898(v=VS.90).aspx
    public class JsonpEndpointBehavior : IEndpointBehavior
    {
        public void Validate(ServiceEndpoint endpoint)
        {
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            var operations = endpointDispatcher.DispatchRuntime.Operations;
            foreach (var operation in operations)
            {
                operation.ParameterInspectors.Add(new Inspector());
            }
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            var operations = clientRuntime.ClientOperations;
            foreach (var operation in operations)
            {
                operation.ParameterInspectors.Add(new Inspector());
            }
        }

        private class Inspector : IParameterInspector
        {
            private const string Callback = "callback";

            public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
            {
            }

            public object BeforeCall(string operationName, object[] inputs)
            {
                var methodName = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters[Callback];
                if (methodName != null)
                {
                    var property = new JsonpMessageProperty { MethodName = methodName };
                    OperationContext.Current.OutgoingMessageProperties.Add(JsonpMessageProperty.Name, property);
                }
                return null;
            }
        }
    }
}