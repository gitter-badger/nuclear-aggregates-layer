using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypePositionSortingOrder : EntityTypeBase<EntityTypePositionSortingOrder>
    {
        public override int Id
        {
            get { return EntityTypeIds.PositionSortingOrder; }
        }

        public override string Description
        {
            get { return "PositionSortingOrder"; }
        }
    }
}