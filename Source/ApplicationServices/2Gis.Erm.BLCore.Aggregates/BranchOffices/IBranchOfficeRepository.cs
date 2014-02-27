using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.BranchOffices.DTO;
using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.BranchOffices
{
    public interface IBranchOfficeRepository : IAggregateRootRepository<BranchOffice>,
                                               IActivateAggregateRepository<BranchOfficeOrganizationUnit>,
                                               IDeactivateAggregateRepository<BranchOffice>,
                                               IDeactivateAggregateRepository<BranchOfficeOrganizationUnit>
    {
        BranchOfficeOrganizationShortInformationDto GetBranchOfficeOrganizationUnitShortInfo(long organizationUnitId);
        IEnumerable<long> GetOrganizationUnitTerritories(long organizationUnit);
        int Deactivate(BranchOffice branchOffice);
        int Deactivate(BranchOfficeOrganizationUnit branchOfficeOrganizationUnit);

        void CreateOrUpdate(BranchOfficeOrganizationUnit branchOfficeOrganizationUnit);
        void CreateOrUpdate(BranchOffice branchOffice);
        void SetPrimaryBranchOfficeOrganizationUnit(long branchOfficeOrganizationUnitId);
        void SetPrimaryForRegionalSalesBranchOfficeOrganizationUnit(long branchOfficeOrganizationUnitId);

        long? GetPrintFormTemplateId(long branchOfficeOrganizationUnitId, TemplateCode templateCode);
        ContributionTypeEnum GetContributionTypeForOrganizationUnit(long organizationUnitId);
    }
}