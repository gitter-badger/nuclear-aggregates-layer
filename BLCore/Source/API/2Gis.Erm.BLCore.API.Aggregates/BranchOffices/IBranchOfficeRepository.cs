using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;

namespace DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices
{
    public interface IBranchOfficeRepository : IAggregateRootService<BranchOffice>,
                                               IActivateAggregateRepository<BranchOffice>,
                                               IActivateAggregateRepository<BranchOfficeOrganizationUnit>,
                                               IDeactivateAggregateRepository<BranchOffice>,
                                               IDeactivateAggregateRepository<BranchOfficeOrganizationUnit>
    {
        IEnumerable<long> GetOrganizationUnitTerritories(long organizationUnit);
        int Deactivate(BranchOffice branchOffice);
        int Deactivate(BranchOfficeOrganizationUnit branchOfficeOrganizationUnit);

        void SetPrimaryBranchOfficeOrganizationUnit(long branchOfficeOrganizationUnitId);
        void SetPrimaryForRegionalSalesBranchOfficeOrganizationUnit(long branchOfficeOrganizationUnitId);

        long? GetPrintFormTemplateId(long branchOfficeOrganizationUnitId, TemplateCode templateCode);
    }
}