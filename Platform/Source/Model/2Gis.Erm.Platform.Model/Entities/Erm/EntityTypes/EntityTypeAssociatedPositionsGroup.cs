using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeAssociatedPositionsGroup : EntityTypeBase<EntityTypeAssociatedPositionsGroup>
    {
        public override int Id
        {
            get { return EntityTypeIds.AssociatedPositionsGroup; }
        }

        public override string Description
        {
            get { return "AssociatedPositionsGroup"; }
        }
    }
}