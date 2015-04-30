using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.BranchOffices.Operations
{
    public class DeleteBranchOfficeOrganizationUnitAggregateService : IAggregateRootRepository<BranchOffice>, IDeleteAggregateRepository<BranchOfficeOrganizationUnit>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISecureRepository<BranchOfficeOrganizationUnit> _branchOfficeOrgUnitSecureRepository;
        private readonly IBranchOfficeReadModel _branchOfficeReadModel;

        public DeleteBranchOfficeOrganizationUnitAggregateService(
            IOperationScopeFactory operationScopeFactory,
            ISecureRepository<BranchOfficeOrganizationUnit> branchOfficeOrgUnitSecureRepository,
            IBranchOfficeReadModel branchOfficeReadModel)
        {
            _operationScopeFactory = operationScopeFactory;
            _branchOfficeOrgUnitSecureRepository = branchOfficeOrgUnitSecureRepository;
            _branchOfficeReadModel = branchOfficeReadModel;
        }

        public int Delete(BranchOfficeOrganizationUnit entity)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, BranchOfficeOrganizationUnit>())
            {
                _branchOfficeOrgUnitSecureRepository.Delete(entity);
                operationScope.Deleted<BranchOfficeOrganizationUnit>(entity.Id);

                var count = _branchOfficeOrgUnitSecureRepository.Save();

                operationScope.Complete();

                return count;
            }
            }

        public int Delete(long entityId)
        {
            return Delete(_branchOfficeReadModel.GetBranchOfficeOrganizationUnit(entityId));
        }
    }
}