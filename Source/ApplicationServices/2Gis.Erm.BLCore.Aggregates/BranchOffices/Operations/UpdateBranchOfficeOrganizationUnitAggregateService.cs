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
    public class UpdateBranchOfficeOrganizationUnitAggregateService : IUpdatePartableEntityAggregateService<BranchOffice, BranchOfficeOrganizationUnit>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISecureRepository<BranchOfficeOrganizationUnit> _branchOfficeOrgUnitSecureRepository;
        private readonly IUpdateDynamicAggregateRepository<BusinessEntityInstance, BusinessEntityPropertyInstance> _updateDynamicAggregateRepository;

        public UpdateBranchOfficeOrganizationUnitAggregateService(
            IOperationScopeFactory operationScopeFactory,
            ISecureRepository<BranchOfficeOrganizationUnit> branchOfficeOrgUnitSecureRepository,
            IUpdateDynamicAggregateRepository<BusinessEntityInstance, BusinessEntityPropertyInstance> updateDynamicAggregateRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _branchOfficeOrgUnitSecureRepository = branchOfficeOrgUnitSecureRepository;
            _updateDynamicAggregateRepository = updateDynamicAggregateRepository;
        }

        public void Update(BranchOfficeOrganizationUnit entity, IEnumerable<BusinessEntityInstanceDto> entityInstanceDtos)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, BranchOfficeOrganizationUnit>())
            {
                _branchOfficeOrgUnitSecureRepository.Update(entity);
                operationScope.Updated<BranchOfficeOrganizationUnit>(entity.Id);

                _branchOfficeOrgUnitSecureRepository.Save();

                foreach (var entityInstanceDto in entityInstanceDtos)
                {
                    _updateDynamicAggregateRepository.Update(entityInstanceDto.EntityInstance, entityInstanceDto.PropertyInstances);
                }

                operationScope.Complete();
            }
        }
    }
}