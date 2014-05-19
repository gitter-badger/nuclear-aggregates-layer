using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.DTO;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel
{
    public interface IBranchOfficeReadModel : IAggregateReadModel<BranchOffice>
    {
        BranchOffice GetBranchOffice(long branchOfficeId);
        BranchOfficeOrganizationUnit GetBranchOfficeOrganizationUnit(long branchOfficeOrganizationUnitId);
        BranchOfficeOrganizationUnit GetBranchOfficeOrganizationUnit(string syncCode1C);

        IEnumerable<BusinessEntityInstanceDto> GetBusinessEntityInstanceDto(BranchOffice branchOffice);
        IEnumerable<BusinessEntityInstanceDto> GetBusinessEntityInstanceDto(BranchOfficeOrganizationUnit branchOfficeOrganizationUnit);

        T GetBranchOfficeDto<T>(long entityId) where T : BranchOfficeDomainEntityDto, new();
        T GetBranchOfficeOrganizationUnitDto<T>(long entityId) where T : BranchOfficeOrganizationUnitDomainEntityDto, new();
        
        IEnumerable<long> GetProjectOrganizationUnitIds(long projectCode);
        ContributionTypeEnum GetOrganizationUnitContributionType(long organizationUnitId);
    }
}