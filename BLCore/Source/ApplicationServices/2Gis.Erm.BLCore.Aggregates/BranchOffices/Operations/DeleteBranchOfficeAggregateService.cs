using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.Aggregates.BranchOffices.Operations
{
    public class DeleteBranchOfficeAggregateService : IAggregateRootRepository<BranchOffice>, IDeleteAggregateRepository<BranchOffice>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<BranchOffice> _branchOfficeRepository;
        private readonly IBranchOfficeReadModel _officeReadModel;
        
        public DeleteBranchOfficeAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<BranchOffice> branchOfficeRepository,
            IBranchOfficeReadModel officeReadModel)
        {
            _operationScopeFactory = operationScopeFactory;
            _branchOfficeRepository = branchOfficeRepository;
            _officeReadModel = officeReadModel;
        }

        public int Delete(BranchOffice entity)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, BranchOffice>())
            {
                _branchOfficeRepository.Delete(entity);
                operationScope.Deleted<BranchOffice>(entity.Id);

                var count = _branchOfficeRepository.Save();

                operationScope.Complete();

                return count;
            }
            }

        public int Delete(long entityId)
        {
            return Delete(_officeReadModel.GetBranchOffice(entityId));
        }
    }
}