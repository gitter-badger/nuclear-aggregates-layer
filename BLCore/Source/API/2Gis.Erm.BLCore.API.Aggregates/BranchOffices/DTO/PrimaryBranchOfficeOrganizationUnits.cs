using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.DTO
{
    public sealed class PrimaryBranchOfficeOrganizationUnits
    {
        public BranchOfficeOrganizationUnit Primary { get; set; }
        public BranchOfficeOrganizationUnit PrimaryForRegionalSales { get; set; }
    }
}