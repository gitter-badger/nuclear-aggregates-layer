using DoubleGis.Erm.BLCore.Aggregates.Common.DTO;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.BranchOffices.ReadModel
{
    public interface IBranchOfficeReadModel : IAggregateReadModel<BranchOffice>
    {
        BranchOfficeOrganizationUnit GetBranchOfficeOrganizationUnit(long branchOfficeOrganizationUnitId);
        BranchOfficeOrganizationUnit GetBranchOfficeOrganizationUnit(string syncCode1C);
        BusinessEntityInstanceDto GetBusinessEntityInstanceDto(BranchOfficeOrganizationUnitPart branchOfficeOrganizationUnitPart);
    }
}