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
    public class DeleteLegalPersonProfileAggregateService : IDeletePartableEntityAggregateService<LegalPerson, LegalPersonProfile>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISecureRepository<LegalPersonProfile> _legalPersonProfileSecureRepository;
        private readonly IDeleteDynamicAggregateRepository<BusinessEntityInstance, BusinessEntityPropertyInstance> _deleteDynamicAggregateRepository;

        public DeleteLegalPersonProfileAggregateService(
            IOperationScopeFactory operationScopeFactory,
            ISecureRepository<LegalPersonProfile> legalPersonProfileSecureRepository,
            IDeleteDynamicAggregateRepository<BusinessEntityInstance, BusinessEntityPropertyInstance> deleteDynamicAggregateRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _legalPersonProfileSecureRepository = legalPersonProfileSecureRepository;
            _deleteDynamicAggregateRepository = deleteDynamicAggregateRepository;
        }

        public void Delete(LegalPersonProfile entity, IEnumerable<BusinessEntityInstanceDto> entityInstanceDtos)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, LegalPersonProfile>())
            {
                foreach (var entityInstanceDto in entityInstanceDtos)
                {
                    _deleteDynamicAggregateRepository.Delete(entityInstanceDto.EntityInstance, entityInstanceDto.PropertyInstances);
                }

                _legalPersonProfileSecureRepository.Delete(entity);
                operationScope.Deleted<LegalPersonProfile>(entity.Id);

                _legalPersonProfileSecureRepository.Save();

                operationScope.Complete();
            }
        }
    }
}