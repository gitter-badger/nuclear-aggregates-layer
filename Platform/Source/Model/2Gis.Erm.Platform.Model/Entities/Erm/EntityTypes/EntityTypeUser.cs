using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeUser : EntityTypeBase<EntityTypeUser>
    {
        public override int Id
        {
            get { return EntityTypeIds.User; }
        }

        public override string Description
        {
            get { return "User"; }
        }
    }
}