using System.Configuration;

namespace DoubleGis.Erm.BLCore.OrderValidation.Settings.Xml
{
    [ConfigurationCollection(typeof(AssociatedDeniedPosition), AddItemName = "position")]
    public sealed class AssociatedDeniedPositions : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new AssociatedDeniedPosition();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AssociatedDeniedPosition)element).Id;
        }
    }
}