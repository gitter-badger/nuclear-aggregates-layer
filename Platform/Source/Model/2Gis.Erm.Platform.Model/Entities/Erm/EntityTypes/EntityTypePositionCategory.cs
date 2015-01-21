using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypePositionCategory : EntityTypeBase<EntityTypePositionCategory>
    {
        public override int Id
        {
            get { return EntityTypeIds.PositionCategory; }
        }

        public override string Description
        {
            get { return "PositionCategory"; }
        }
    }
}