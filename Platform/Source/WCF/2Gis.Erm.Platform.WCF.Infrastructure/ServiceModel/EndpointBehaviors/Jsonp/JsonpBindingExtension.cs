using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.EndpointBehaviors.Jsonp
{
    public class JsonpBindingExtension : BindingElementExtensionElement
    {
        public override Type BindingElementType
        {
            get { return typeof(JsonpBindingElement); }

        }

        protected override BindingElement CreateBindingElement()
        {
            return new JsonpBindingElement();
        }
    }
}