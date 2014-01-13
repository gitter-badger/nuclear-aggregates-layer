using System.Configuration;

using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.OrderValidation.Configuration
{
    internal sealed class AssociatedDeniedPosition : ConfigurationElement
    {
        [ConfigurationProperty("id", IsKey = true, IsRequired = true)]
        public long Id
        {
            get
            {
                return (long)this["id"];
            }

            set
            {
                this["id"] = value;
            }
        }

        [ConfigurationProperty("bindingType", IsKey = false, IsRequired = false)]
        public ObjectBindingType BindingType
        {
            get
            {
                return (ObjectBindingType)this["bindingType"];
            }

            set
            {
                this["bindingType"] = value;
            }
        }

        [ConfigurationProperty("masterPositions")]
        public AssociatedDeniedPositions MasterPositions
        {
            get { return (AssociatedDeniedPositions)this["masterPositions"]; }
        }

        [ConfigurationProperty("deniedPositions")]
        public AssociatedDeniedPositions DeniedPositions
        {
            get { return (AssociatedDeniedPositions)this["deniedPositions"]; }
        }
    }
}