using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeOrganizationUnit : EntityTypeBase<EntityTypeOrganizationUnit>
    {
        public override int Id
        {
            get { return EntityTypeIds.OrganizationUnit; }
        }

        public override string Description
        {
            get { return "OrganizationUnit"; }
        }
    }
}