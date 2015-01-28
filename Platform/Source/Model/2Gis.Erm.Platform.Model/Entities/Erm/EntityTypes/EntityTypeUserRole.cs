using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeUserRole : EntityTypeBase<EntityTypeUserRole>
    {
        public override int Id
        {
            get { return EntityTypeIds.UserRole; }
        }

        public override string Description
        {
            get { return "UserRole"; }
        }
    }
}