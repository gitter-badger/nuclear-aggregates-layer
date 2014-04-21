using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.Common.DTO;
using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.Aggregates.Dynamic.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.BranchOffices.Operations
{
    public class DeleteBranchOfficeAggregateService : IDeletePartableEntityAggregateService<BranchOffice, BranchOffice>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<BranchOffice> _branchOfficeRepository;
        private readonly IDeleteDynamicAggregateRepository<BusinessEntityInstance, BusinessEntityPropertyInstance> _deleteDynamicAggregateRepository;
        
        public DeleteBranchOfficeAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<BranchOffice> branchOfficeRepository,
            IDeleteDynamicAggregateRepository<BusinessEntityInstance, BusinessEntityPropertyInstance> deleteDynamicAggregateRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _branchOfficeRepository = branchOfficeRepository;
            _deleteDynamicAggregateRepository = deleteDynamicAggregateRepository;
        }

        public void Delete(BranchOffice entity, IEnumerable<BusinessEntityInstanceDto> entityInstanceDtos)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, BranchOffice>())
            {
                foreach (var entityInstanceDto in entityInstanceDtos)
                {
                    _deleteDynamicAggregateRepository.Delete(entityInstanceDto.EntityInstance, entityInstanceDto.PropertyInstances);
                }

                _branchOfficeRepository.Delete(entity);
                operationScope.Deleted<BranchOffice>(entity.Id);

                _branchOfficeRepository.Save();

                operationScope.Complete();
            }
        }
    }
}