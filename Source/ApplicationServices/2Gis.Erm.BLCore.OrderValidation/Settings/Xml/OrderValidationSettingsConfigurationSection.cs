using System.Configuration;

namespace DoubleGis.Erm.BLCore.OrderValidation.Settings.Xml
{
    public sealed class OrderValidationSettingsConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("associatedDeniedPositions")]
        public AssociatedDeniedPositions AssociatedDeniedPositions
        {
            get { return (AssociatedDeniedPositions)this["associatedDeniedPositions"]; }
        }
    }
}