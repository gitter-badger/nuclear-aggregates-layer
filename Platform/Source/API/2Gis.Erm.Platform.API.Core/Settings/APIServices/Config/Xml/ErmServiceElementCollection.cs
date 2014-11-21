using System.Configuration;

namespace DoubleGis.Erm.Platform.API.Core.Settings.APIServices.Config.Xml
{
    public sealed class ErmServiceElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ErmServiceElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ErmServiceElement)element).ServiceName;
        }
    }
}