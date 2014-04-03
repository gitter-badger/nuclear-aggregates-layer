using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.Common.DTO;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.BranchOffices.ReadModel
{
    public interface IBranchOfficeReadModel : IAggregateReadModel<BranchOffice>
    {
        BranchOfficeOrganizationUnit GetBranchOfficeOrganizationUnit(long branchOfficeOrganizationUnitId);
        BranchOfficeOrganizationUnit GetBranchOfficeOrganizationUnit(string syncCode1C);
        BusinessEntityInstanceDto GetBusinessEntityInstanceDto(BranchOfficeOrganizationUnitPart branchOfficeOrganizationUnitPart);
        IEnumerable<long> GetProjectOrganizationUnitIds(long projectCode);
        ContributionTypeEnum GetOrganizationUnitContributionType(long organizationUnitId);
    }
}