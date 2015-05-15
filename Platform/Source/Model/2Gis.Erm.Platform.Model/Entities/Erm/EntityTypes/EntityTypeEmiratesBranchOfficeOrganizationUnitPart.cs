using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.EntityTypes
{
    public class EntityTypeEmiratesBranchOfficeOrganizationUnitPart : EntityTypeBase<EntityTypeEmiratesBranchOfficeOrganizationUnitPart>
    {
        public override int Id
        {
            get { return EntityTypeIds.EmiratesBranchOfficeOrganizationUnitPart; }
        }

        public override string Description
        {
            get { return "EmiratesBranchOfficeOrganizationUnitPart"; }
        }
    }
}