using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeOrderReleaseTotal : EntityTypeBase<EntityTypeOrderReleaseTotal>
    {
        public override int Id
        {
            get { return EntityTypeIds.OrderReleaseTotal; }
        }

        public override string Description
        {
            get { return "OrderReleaseTotal"; }
        }
    }
}