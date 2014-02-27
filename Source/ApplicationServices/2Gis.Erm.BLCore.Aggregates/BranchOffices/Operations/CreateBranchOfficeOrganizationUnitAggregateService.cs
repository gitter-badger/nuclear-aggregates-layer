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
    public class CreateBranchOfficeOrganizationUnitAggregateService : ICreatePartableEntityAggregateService<BranchOffice, BranchOfficeOrganizationUnit>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISecureRepository<BranchOfficeOrganizationUnit> _branchOfficeOrgUnitSecureRepository;
        private readonly ICreateDynamicAggregateRepository<BusinessEntityInstance, BusinessEntityPropertyInstance> _createDynamicAggregateRepository;

        public CreateBranchOfficeOrganizationUnitAggregateService(
            IOperationScopeFactory operationScopeFactory,
            ISecureRepository<BranchOfficeOrganizationUnit> branchOfficeOrgUnitSecureRepository,
            ICreateDynamicAggregateRepository<BusinessEntityInstance, BusinessEntityPropertyInstance> createDynamicAggregateRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _branchOfficeOrgUnitSecureRepository = branchOfficeOrgUnitSecureRepository;
            _createDynamicAggregateRepository = createDynamicAggregateRepository;
        }

        public long Create(BranchOfficeOrganizationUnit entity, IEnumerable<BusinessEntityInstanceDto> entityInstanceDtos)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, BranchOfficeOrganizationUnit>())
            {
                _branchOfficeOrgUnitSecureRepository.Add(entity);
                operationScope.Added<BranchOfficeOrganizationUnit>(entity.Id);

                _branchOfficeOrgUnitSecureRepository.Save();

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