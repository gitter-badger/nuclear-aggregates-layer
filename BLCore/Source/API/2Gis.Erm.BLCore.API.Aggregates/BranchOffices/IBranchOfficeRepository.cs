using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices
{
    public interface IBranchOfficeRepository : IAggregateRootRepository<BranchOffice>,
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