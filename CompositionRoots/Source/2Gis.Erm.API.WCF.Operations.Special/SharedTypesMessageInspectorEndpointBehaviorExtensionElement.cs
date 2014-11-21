using System;
using System.ServiceModel.Configuration;

using DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.EndpointBehaviors.SharedTypes;

namespace DoubleGis.Erm.API.WCF.Operations.Special
{
    public class SharedTypesMessageInspectorEndpointBehaviorExtensionElement : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(SharedTypesMessageInspectorEndpointBehavior); }
        }

        protected override object CreateBehavior()
        {
            var typeNameConverter = new SoapTypeNameConveter();
            var inspector = new SharedTypesMessageInspector(typeNameConverter, SharedTypesProvider.NamespacesByAssemblies);
            return new SharedTypesMessageInspectorEndpointBehavior(inspector);
        }
    }
}