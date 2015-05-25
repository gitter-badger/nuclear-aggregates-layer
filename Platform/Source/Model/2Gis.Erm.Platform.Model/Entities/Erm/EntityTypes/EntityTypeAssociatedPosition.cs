using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeAssociatedPosition : EntityTypeBase<EntityTypeAssociatedPosition>
    {
        public override int Id
        {
            get { return EntityTypeIds.AssociatedPosition; }
        }

        public override string Description
        {
            get { return "AssociatedPosition"; }
        }
    }
}