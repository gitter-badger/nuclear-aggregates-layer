using System;
using System.ServiceModel.Configuration;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.EndpointBehaviors.Jsonp
{
    public class JsonpBehaviorExtension : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(JsonpEndpointBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new JsonpEndpointBehavior();
        }
    }
}