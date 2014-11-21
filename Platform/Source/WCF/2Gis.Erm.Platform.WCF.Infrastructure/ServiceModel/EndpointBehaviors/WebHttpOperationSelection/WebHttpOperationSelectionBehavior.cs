using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.EndpointBehaviors.WebHttpOperationSelection
{
    public class WebHttpOperationSelectionBehavior : WebHttpBehavior
    {
        protected override WebHttpDispatchOperationSelector GetOperationSelector(ServiceEndpoint endpoint)
        {
            return new WebHttpDispatchOperationWithOptionalParamsSelector(endpoint);
        }

        private class WebHttpDispatchOperationWithOptionalParamsSelector : WebHttpDispatchOperationSelector
        {
            private const string AdditionalSymbol = "null";

            public WebHttpDispatchOperationWithOptionalParamsSelector(ServiceEndpoint endpoint) : base(endpoint)
            {
            }

            protected override string SelectOperation(ref Message message, out bool uriMatched)
            {
                var originalAddress = message.Headers.To.AbsoluteUri;
                
                var result = base.SelectOperation(ref message, out uriMatched);
                for (var i = 0; !uriMatched && i < 5; i++)
                {
                    var address = message.Headers.To.AbsoluteUri;
                    address = address.EndsWith("/") ? address.Substring(0, address.Length - 1) : string.Format("{0}/{1}", address, AdditionalSymbol);

                    message.Headers.To = new Uri(address);
                    result = base.SelectOperation(ref message, out uriMatched);
                }

                if (!uriMatched)
                {
                    message.Headers.To = new Uri(originalAddress);
                }

                return result;
            }
        }
    }
}