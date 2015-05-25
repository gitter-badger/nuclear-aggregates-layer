using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeChileBranchOfficeOrganizationUnitPart : EntityTypeBase<EntityTypeChileBranchOfficeOrganizationUnitPart>
    {
        public override int Id
        {
            get { return EntityTypeIds.ChileBranchOfficeOrganizationUnitPart; }
        }

        public override string Description
        {
            get { return "ChileBranchOfficeOrganizationUnitPart"; }
        }
    }
}