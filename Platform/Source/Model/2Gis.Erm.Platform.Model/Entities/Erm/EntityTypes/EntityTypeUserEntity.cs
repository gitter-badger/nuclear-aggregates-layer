using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeUserEntity : EntityTypeBase<EntityTypeUserEntity>
    {
        public override int Id
        {
            get { return EntityTypeIds.UserEntity; }
        }

        public override string Description
        {
            get { return "UserEntity"; }
        }
    }
}