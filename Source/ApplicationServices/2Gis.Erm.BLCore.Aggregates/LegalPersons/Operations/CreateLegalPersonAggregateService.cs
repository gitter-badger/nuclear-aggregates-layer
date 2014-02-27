using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.Common.DTO;
using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.Aggregates.Dynamic.Operations;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.LegalPersons.Operations
{
    public class CreateLegalPersonAggregateService : ICreatePartableEntityAggregateService<LegalPerson, LegalPerson>
    {
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISecureRepository<LegalPerson> _legalPersonSecureRepository;
        private readonly ICreateDynamicAggregateRepository<BusinessEntityInstance, BusinessEntityPropertyInstance> _createDynamicAggregateRepository;

        public CreateLegalPersonAggregateService(
            IIdentityProvider identityProvider,
            IOperationScopeFactory operationScopeFactory,
            ISecureRepository<LegalPerson> legalPersonSecureRepository,
            ICreateDynamicAggregateRepository<BusinessEntityInstance, BusinessEntityPropertyInstance> createDynamicAggregateRepository)
        {
            _identityProvider = identityProvider;
            _operationScopeFactory = operationScopeFactory;
            _legalPersonSecureRepository = legalPersonSecureRepository;
            _createDynamicAggregateRepository = createDynamicAggregateRepository;
        }

        public long Create(LegalPerson entity, IEnumerable<BusinessEntityInstanceDto> entityInstanceDtos)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, LegalPerson>())
            {
                _identityProvider.SetFor(entity);
                _legalPersonSecureRepository.Add(entity);
                operationScope.Added<LegalPerson>(entity.Id);
                
                _legalPersonSecureRepository.Save();

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