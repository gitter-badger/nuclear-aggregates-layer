using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.BranchOffices.Operations
{
    public class CreateBranchOfficeOrganizationUnitAggregateService : IAggregateRootRepository<BranchOffice>, ICreateAggregateRepository<BranchOfficeOrganizationUnit>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISecureRepository<BranchOfficeOrganizationUnit> _branchOfficeOrgUnitSecureRepository;

        public CreateBranchOfficeOrganizationUnitAggregateService(IOperationScopeFactory operationScopeFactory,
                                                                  ISecureRepository<BranchOfficeOrganizationUnit> branchOfficeOrgUnitSecureRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _branchOfficeOrgUnitSecureRepository = branchOfficeOrgUnitSecureRepository;
        }

        public int Create(BranchOfficeOrganizationUnit entity)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, BranchOfficeOrganizationUnit>())
            {
                _branchOfficeOrgUnitSecureRepository.Add(entity);
                operationScope.Added<BranchOfficeOrganizationUnit>(entity.Id);

                var count = _branchOfficeOrgUnitSecureRepository.Save();

                operationScope.Complete();

                return count;
            }
        }
    }
}