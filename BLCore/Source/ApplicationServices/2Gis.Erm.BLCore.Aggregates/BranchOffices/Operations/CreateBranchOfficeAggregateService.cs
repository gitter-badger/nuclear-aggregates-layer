using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.Aggregates.BranchOffices.Operations
{
    public class CreateBranchOfficeAggregateService : IAggregateRootService<BranchOffice>, ICreateAggregateRepository<BranchOffice>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<BranchOffice> _branchOfficeRepository;

        public CreateBranchOfficeAggregateService(IOperationScopeFactory operationScopeFactory,
                                                  IRepository<BranchOffice> branchOfficeRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _branchOfficeRepository = branchOfficeRepository;
        }

        public int Create(BranchOffice entity)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, BranchOffice>())
            {
                _branchOfficeRepository.Add(entity);
                operationScope.Added<BranchOffice>(entity.Id);
                
                var count = _branchOfficeRepository.Save();
                
                operationScope.Complete();

                return count;
            }
        }
    }
}
