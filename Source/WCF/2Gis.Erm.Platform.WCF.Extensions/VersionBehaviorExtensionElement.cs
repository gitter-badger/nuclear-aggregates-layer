using System;
using System.Configuration;
using System.ServiceModel.Configuration;

namespace DoubleGis.Erm.Platform.WCF.Extensions
{
    public sealed class VersionBehaviorExtensionElement : BehaviorExtensionElement 
    {
        public override Type BehaviorType
        {
            get { return typeof(VersionBehavior); }
        }

        protected override object CreateBehavior()
        {
            // Если в конфиге данной настройки нет, то и в сообщение ничего добавлено не будет.
            var adaptation = ConfigurationManager.AppSettings["BusinessLogicAdaptation"];
            return new VersionBehavior(adaptation);
        }
    }
}
