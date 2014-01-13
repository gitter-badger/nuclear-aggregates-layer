using System.Configuration;

namespace DoubleGis.Erm.BLCore.OrderValidation.Configuration
{
    internal sealed class OrderValidationSettingsConfigurationSection : ConfigurationSection
    {
        private static OrderValidationSettingsConfigurationSection _instance;

        public static OrderValidationSettingsConfigurationSection Instance
        {
            get
            {
                return _instance ??
                    (_instance = (OrderValidationSettingsConfigurationSection)ConfigurationManager.GetSection("orderValidationSettings"));
            }
        }

        [ConfigurationProperty("associatedDeniedPositions")]
        public AssociatedDeniedPositions AssociatedDeniedPositions
        {
            get { return (AssociatedDeniedPositions)this["associatedDeniedPositions"]; }
        }
    }
}