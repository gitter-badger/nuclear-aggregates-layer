using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.BranchOffices.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.DTO;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel
{
    public interface IBranchOfficeReadModel : IAggregateReadModel<BranchOffice>
    {
        BranchOffice GetBranchOffice(long branchOfficeId);

        BranchOfficeOrganizationUnit GetBranchOfficeOrganizationUnit(long branchOfficeOrganizationUnitId);
        BranchOfficeOrganizationUnit GetBranchOfficeOrganizationUnit(string syncCode1C);
        string GetNameOfActiveDuplicateByInn(long branchOfficeId, string inn);

        bool AreThereAnyActiveInnDuplicates(long branchOfficeId, string inn);

        IEnumerable<long> GetProjectOrganizationUnitIds(long projectCode);
        ContributionTypeEnum GetOrganizationUnitContributionType(long organizationUnitId);
        ContributionTypeEnum GetBranchOfficeOrganizationUnitContributionType(long branchOfficeOrganizationUnitId);

        PrimaryBranchOfficeOrganizationUnits GetPrimaryBranchOfficeOrganizationUnits(long organizationUnitId);

        BranchOfficeOrganizationUnitNamesDto GetBranchOfficeOrganizationUnitDuplicate(long organizationUnitId,
                                                                                      long branchOfficeId,
                                                                                      long branchOfficeOrganizationUnitId);
        
        string GetBranchOfficeName(long branchOfficeId);
        string GetBranchOfficeOrganizationName(long branchOfficeOrganizationUnitId);
        int? GetBranchOfficeOrganizationDgppid(long branchOfficeOrganizationUnitId);
        long GetBargainTypeId(long branchOfficeOrganizationUnitId);
        BranchOfficeOrganizationShortLegalNameDto GetPrimaryBranchOfficeOrganizationUnitName(long organizationUnitId);
        IReadOnlyCollection<BranchOfficeOrganizationShortLegalNameDto> GetBranchOfficeOrganizationUnitNames(long? organizationUnitId, IEnumerable<long> branchOfficeIds);
    }
}