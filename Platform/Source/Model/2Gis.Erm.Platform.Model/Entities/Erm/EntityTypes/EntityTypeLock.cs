using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeLock : EntityTypeBase<EntityTypeLock>
    {
        public override int Id
        {
            get { return EntityTypeIds.Lock; }
        }

        public override string Description
        {
            get { return "Lock"; }
        }
    }
}