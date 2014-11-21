using System;
using System.ServiceModel.Configuration;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.EndpointBehaviors.Json
{
    public sealed class JsonResponseFormatterBehaviorExtension : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(JsonResponseFormatterEndpointBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new JsonResponseFormatterEndpointBehavior();
        }
    }
}
