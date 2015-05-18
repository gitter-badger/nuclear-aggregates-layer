using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeBranchOfficeOrganizationUnit : EntityTypeBase<EntityTypeBranchOfficeOrganizationUnit>
    {
        public override int Id
        {
            get { return EntityTypeIds.BranchOfficeOrganizationUnit; }
        }

        public override string Description
        {
            get { return "BranchOfficeOrganizationUnit"; }
        }
    }
}