using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.Common.DTO;
using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.Aggregates.Dynamic.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.BranchOffices.Operations
{
    public class CreateBranchOfficeAggregateService : ICreatePartableEntityAggregateService<BranchOffice, BranchOffice>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<BranchOffice> _branchOfficeRepository;
        private readonly ICreateDynamicAggregateRepository<BusinessEntityInstance, BusinessEntityPropertyInstance> _createDynamicAggregateRepository;

        public CreateBranchOfficeAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<BranchOffice> branchOfficeRepository,
            ICreateDynamicAggregateRepository<BusinessEntityInstance, BusinessEntityPropertyInstance> createDynamicAggregateRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _branchOfficeRepository = branchOfficeRepository;
            _createDynamicAggregateRepository = createDynamicAggregateRepository;
        }

        public long Create(BranchOffice entity, IEnumerable<BusinessEntityInstanceDto> entityInstanceDtos)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity>(EntityName.BranchOffice))
            {
                _branchOfficeRepository.Add(entity);
                operationScope.Added<BranchOffice>(entity.Id);
                
                _branchOfficeRepository.Save();

                foreach (var entityInstanceDto in entityInstanceDtos)
                {
                    entityInstanceDto.EntityInstance.EntityId = entity.Id;
                    _createDynamicAggregateRepository.Create(entityInstanceDto.EntityInstance, entityInstanceDto.PropertyInstances);
                }
                
                operationScope.Complete();

                return entity.Id;
            }
        }
    }
}
