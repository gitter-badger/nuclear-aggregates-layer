using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypePositionChildren : EntityTypeBase<EntityTypePositionChildren>
    {
        public override int Id
        {
            get { return EntityTypeIds.PositionChildren; }
        }

        public override string Description
        {
            get { return "PositionChildren"; }
        }
    }
}