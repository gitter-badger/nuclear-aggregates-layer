using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Dynamic.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.BranchOffices.Operations
{
    public class DeleteBranchOfficeOrganizationUnitAggregateService : IDeletePartableEntityAggregateService<BranchOffice, BranchOfficeOrganizationUnit>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISecureRepository<BranchOfficeOrganizationUnit> _branchOfficeOrgUnitSecureRepository;
        private readonly IDeleteDynamicAggregateRepository<BusinessEntityInstance, BusinessEntityPropertyInstance> _deleteDynamicAggregateRepository;

        public DeleteBranchOfficeOrganizationUnitAggregateService(
            IOperationScopeFactory operationScopeFactory,
            ISecureRepository<BranchOfficeOrganizationUnit> branchOfficeOrgUnitSecureRepository,
            IDeleteDynamicAggregateRepository<BusinessEntityInstance, BusinessEntityPropertyInstance> deleteDynamicAggregateRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _branchOfficeOrgUnitSecureRepository = branchOfficeOrgUnitSecureRepository;
            _deleteDynamicAggregateRepository = deleteDynamicAggregateRepository;
        }

        public void Delete(BranchOfficeOrganizationUnit entity, IEnumerable<BusinessEntityInstanceDto> entityInstanceDtos)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, BranchOfficeOrganizationUnit>())
            {
                foreach (var entityInstanceDto in entityInstanceDtos)
                {
                    _deleteDynamicAggregateRepository.Delete(entityInstanceDto.EntityInstance, entityInstanceDto.PropertyInstances);
                }

                _branchOfficeOrgUnitSecureRepository.Delete(entity);
                operationScope.Deleted<BranchOfficeOrganizationUnit>(entity.Id);

                _branchOfficeOrgUnitSecureRepository.Save();

                operationScope.Complete();
            }
        }
    }
}