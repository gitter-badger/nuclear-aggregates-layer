using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeFirmAddress : EntityTypeBase<EntityTypeFirmAddress>
    {
        public override int Id
        {
            get { return EntityTypeIds.FirmAddress; }
        }

        public override string Description
        {
            get { return "FirmAddress"; }
        }
    }
}