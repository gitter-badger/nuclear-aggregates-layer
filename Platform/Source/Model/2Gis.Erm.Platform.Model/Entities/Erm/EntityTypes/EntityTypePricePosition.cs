using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypePricePosition : EntityTypeBase<EntityTypePricePosition>
    {
        public override int Id
        {
            get { return EntityTypeIds.PricePosition; }
        }

        public override string Description
        {
            get { return "PricePosition"; }
        }
    }
}