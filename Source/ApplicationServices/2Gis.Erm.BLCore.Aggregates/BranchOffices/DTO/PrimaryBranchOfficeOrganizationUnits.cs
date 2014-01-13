using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.BranchOffices.DTO
{
    internal sealed class PrimaryBranchOfficeOrganizationUnits
    {
        public BranchOfficeOrganizationUnit Primary { get; set; }
        public BranchOfficeOrganizationUnit PrimaryForRegionalSales { get; set; }
    }
}