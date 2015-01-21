using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeUserOrganizationUnit : EntityTypeBase<EntityTypeUserOrganizationUnit>
    {
        public override int Id
        {
            get { return EntityTypeIds.UserOrganizationUnit; }
        }

        public override string Description
        {
            get { return "UserOrganizationUnit"; }
        }
    }
}