using System;
using System.ServiceModel.Configuration;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.EndpointBehaviors.WebHttpOperationSelection
{
    public class OperationSelectionBehaviorExtension : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(WebHttpOperationSelectionBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new WebHttpOperationSelectionBehavior();
        }
    }
}