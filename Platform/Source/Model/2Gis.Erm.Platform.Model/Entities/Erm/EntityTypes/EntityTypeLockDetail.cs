using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeLockDetail : EntityTypeBase<EntityTypeLockDetail>
    {
        public override int Id
        {
            get { return EntityTypeIds.LockDetail; }
        }

        public override string Description
        {
            get { return "LockDetail"; }
        }
    }
}