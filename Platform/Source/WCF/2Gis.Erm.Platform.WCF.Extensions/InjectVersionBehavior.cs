using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Xml.Linq;

namespace DoubleGis.Erm.Platform.WCF.Extensions
{
    public sealed class InjectVersionBehavior : IEndpointBehavior, IDispatchMessageInspector
    {
        private readonly IDictionary<Tuple<Uri, XName>, Version> _versions;
        private readonly string _adaptation;

        public InjectVersionBehavior(IDictionary<Tuple<Uri, XName>, Version> versions, string adaptation)
        {
            _versions = versions;
            _adaptation = adaptation;
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(this);
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            var versionExtendedReply = new VersionExtendedMessage(reply);
            versionExtendedReply.InjectEndpointVersions(_versions, _adaptation);
            reply = versionExtendedReply;
        }
    }
}
