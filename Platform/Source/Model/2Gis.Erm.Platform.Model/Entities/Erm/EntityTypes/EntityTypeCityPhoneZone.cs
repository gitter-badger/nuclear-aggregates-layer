using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeCityPhoneZone : EntityTypeBase<EntityTypeCityPhoneZone>
    {
        public override int Id
        {
            get { return EntityTypeIds.CityPhoneZone; }
        }

        public override string Description
        {
            get { return "CityPhoneZone"; }
        }
    }
}