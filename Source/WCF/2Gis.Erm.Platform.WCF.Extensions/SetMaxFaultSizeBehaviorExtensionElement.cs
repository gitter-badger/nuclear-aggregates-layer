using System;
using System.Configuration;
using System.ServiceModel.Configuration;

namespace DoubleGis.Erm.Platform.WCF.Extensions
{
    public class SetMaxFaultSizeBehaviorExtensionElement : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(SetMaxFaultSizeBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new SetMaxFaultSizeBehavior(Value);
        }

        [ConfigurationProperty("value")]
        private int Value
        {
            get { return (int)this["value"]; }
        }
    }
}