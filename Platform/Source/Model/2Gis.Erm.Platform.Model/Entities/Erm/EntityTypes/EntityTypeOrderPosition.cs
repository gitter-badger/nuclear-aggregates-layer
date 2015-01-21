using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeOrderPosition : EntityTypeBase<EntityTypeOrderPosition>
    {
        public override int Id
        {
            get { return EntityTypeIds.OrderPosition; }
        }

        public override string Description
        {
            get { return "OrderPosition"; }
        }
    }
}