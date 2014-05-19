using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Dynamic.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.BranchOffices.Operations
{
    public class UpdateBranchOfficeAggregateService : IUpdatePartableEntityAggregateService<BranchOffice, BranchOffice>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<BranchOffice> _branchOfficeRepository;
        private readonly IUpdateDynamicAggregateRepository<BusinessEntityInstance, BusinessEntityPropertyInstance> _updateDynamicAggregateRepository;

        public UpdateBranchOfficeAggregateService(
            IOperationScopeFactory operationScopeFactory,
            IRepository<BranchOffice> branchOfficeRepository,
            IUpdateDynamicAggregateRepository<BusinessEntityInstance, BusinessEntityPropertyInstance> updateDynamicAggregateRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _branchOfficeRepository = branchOfficeRepository;
            _updateDynamicAggregateRepository = updateDynamicAggregateRepository;
        }

        public void Update(BranchOffice entity, IEnumerable<BusinessEntityInstanceDto> entityInstanceDtos)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity>(EntityName.BranchOffice))
            {
                _branchOfficeRepository.Update(entity);
                operationScope.Updated<BranchOffice>(entity.Id);

                _branchOfficeRepository.Save();

                foreach (var entityInstanceDto in entityInstanceDtos)
                {
                    _updateDynamicAggregateRepository.Update(entityInstanceDto.EntityInstance, entityInstanceDto.PropertyInstances);
                }

                operationScope.Complete();
            }
        }
    }
}