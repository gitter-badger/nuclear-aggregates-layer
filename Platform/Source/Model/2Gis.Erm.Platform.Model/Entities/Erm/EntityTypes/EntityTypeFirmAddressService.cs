using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeFirmAddressService : EntityTypeBase<EntityTypeFirmAddressService>
    {
        public override int Id
        {
            get { return EntityTypeIds.FirmAddressService; }
        }

        public override string Description
        {
            get { return "FirmAddressService"; }
        }
    }
}