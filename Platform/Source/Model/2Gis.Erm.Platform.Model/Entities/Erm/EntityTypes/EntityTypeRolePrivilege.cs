using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeRolePrivilege : EntityTypeBase<EntityTypeRolePrivilege>
    {
        public override int Id
        {
            get { return EntityTypeIds.RolePrivilege; }
        }

        public override string Description
        {
            get { return "RolePrivilege"; }
        }
    }
}