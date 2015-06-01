using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage.Writings;

namespace DoubleGis.Erm.BLCore.Aggregates.BranchOffices.Operations
{
    public class UpdateBranchOfficeAggregateService : IAggregateRootService<BranchOffice>, IUpdateAggregateRepository<BranchOffice>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<BranchOffice> _branchOfficeRepository;

        public UpdateBranchOfficeAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<BranchOffice> branchOfficeRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _branchOfficeRepository = branchOfficeRepository;
        }

        public int Update(BranchOffice entity)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, BranchOffice>())
            {
                // Проверка на наличие дублей по реквизитам НЕ нужна
                _branchOfficeRepository.Update(entity);
                operationScope.Updated<BranchOffice>(entity.Id);

                var count = _branchOfficeRepository.Save();

                operationScope.Complete();
                return count;
            }
        }
    }
}