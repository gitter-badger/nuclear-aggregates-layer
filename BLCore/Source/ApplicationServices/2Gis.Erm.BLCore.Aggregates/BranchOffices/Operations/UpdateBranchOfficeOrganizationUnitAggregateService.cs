using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.BranchOffices.Operations
{
    public class UpdateBranchOfficeOrganizationUnitAggregateService : IAggregateRootRepository<BranchOffice>, IUpdateAggregateRepository<BranchOfficeOrganizationUnit>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISecureRepository<BranchOfficeOrganizationUnit> _branchOfficeOrgUnitSecureRepository;

        public UpdateBranchOfficeOrganizationUnitAggregateService(
            IOperationScopeFactory operationScopeFactory,
            ISecureRepository<BranchOfficeOrganizationUnit> branchOfficeOrgUnitSecureRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _branchOfficeOrgUnitSecureRepository = branchOfficeOrgUnitSecureRepository;
        }

        public int Update(BranchOfficeOrganizationUnit entity)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, BranchOfficeOrganizationUnit>())
            {
                _branchOfficeOrgUnitSecureRepository.Update(entity);
                operationScope.Updated<BranchOfficeOrganizationUnit>(entity.Id);

                var count = _branchOfficeOrgUnitSecureRepository.Save();

                operationScope.Complete();

                return count;
            }
        }
    }
}