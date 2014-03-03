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
    public class CreateLegalPersonProfileAggregateService : ICreatePartableEntityAggregateService<LegalPerson, LegalPersonProfile>
    {
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISecureRepository<LegalPersonProfile> _legalPersonProfileSecureRepository;
        private readonly ICreateDynamicAggregateRepository<BusinessEntityInstance, BusinessEntityPropertyInstance> _createDynamicAggregateRepository;

        public CreateLegalPersonProfileAggregateService(
            IIdentityProvider identityProvider,
            IOperationScopeFactory operationScopeFactory,
            ISecureRepository<LegalPersonProfile> legalPersonProfileSecureRepository,
            ICreateDynamicAggregateRepository<BusinessEntityInstance, BusinessEntityPropertyInstance> createDynamicAggregateRepository)
        {
            _identityProvider = identityProvider;
            _operationScopeFactory = operationScopeFactory;
            _legalPersonProfileSecureRepository = legalPersonProfileSecureRepository;
            _createDynamicAggregateRepository = createDynamicAggregateRepository;
        }

        public long Create(LegalPersonProfile entity, IEnumerable<BusinessEntityInstanceDto> entityInstanceDtos)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, LegalPersonProfile>())
            {
                _identityProvider.SetFor(entity);
                _legalPersonProfileSecureRepository.Add(entity);
                operationScope.Added<LegalPersonProfile>(entity.Id);

                _legalPersonProfileSecureRepository.Save();

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