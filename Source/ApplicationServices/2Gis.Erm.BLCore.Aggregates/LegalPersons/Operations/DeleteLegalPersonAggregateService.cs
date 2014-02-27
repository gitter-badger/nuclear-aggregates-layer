using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.Common.DTO;
using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.Aggregates.Dynamic.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.LegalPersons.Operations
{
    public class DeleteLegalPersonAggregateService : IDeletePartableEntityAggregateService<LegalPerson, LegalPerson>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISecureRepository<LegalPerson> _legalPersonSecureRepository;
        private readonly IDeleteDynamicAggregateRepository<BusinessEntityInstance, BusinessEntityPropertyInstance> _deleteDynamicAggregateRepository;

        public DeleteLegalPersonAggregateService(
            IOperationScopeFactory operationScopeFactory,
            ISecureRepository<LegalPerson> legalPersonSecureRepository,
            IDeleteDynamicAggregateRepository<BusinessEntityInstance, BusinessEntityPropertyInstance> deleteDynamicAggregateRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _legalPersonSecureRepository = legalPersonSecureRepository;
            _deleteDynamicAggregateRepository = deleteDynamicAggregateRepository;
        }

        public void Delete(LegalPerson entity, IEnumerable<BusinessEntityInstanceDto> entityInstanceDtos)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, LegalPerson>())
            {
                foreach (var entityInstanceDto in entityInstanceDtos)
                {
                    _deleteDynamicAggregateRepository.Delete(entityInstanceDto.EntityInstance, entityInstanceDto.PropertyInstances);
                }

                _legalPersonSecureRepository.Delete(entity);
                operationScope.Deleted<LegalPerson>(entity.Id);

                _legalPersonSecureRepository.Save();

                operationScope.Complete();
            }
        }
    }
}