using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Dynamic.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.LegalPersons.Operations
{
    public class UpdateLegalPersonProfileAggregateService : IUpdatePartableEntityAggregateService<LegalPerson, LegalPersonProfile>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISecureRepository<LegalPersonProfile> _legalPersonProfileSecureRepository;
        private readonly IUpdateDynamicAggregateRepository<BusinessEntityInstance, BusinessEntityPropertyInstance> _updateDynamicAggregateRepository;

        public UpdateLegalPersonProfileAggregateService(
            IOperationScopeFactory operationScopeFactory,
            ISecureRepository<LegalPersonProfile> legalPersonProfileSecureRepository,
            IUpdateDynamicAggregateRepository<BusinessEntityInstance, BusinessEntityPropertyInstance> updateDynamicAggregateRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _legalPersonProfileSecureRepository = legalPersonProfileSecureRepository;
            _updateDynamicAggregateRepository = updateDynamicAggregateRepository;
        }

        public void Update(LegalPersonProfile entity, IEnumerable<BusinessEntityInstanceDto> entityInstanceDtos)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, LegalPersonProfile>())
            {
                _legalPersonProfileSecureRepository.Update(entity);
                operationScope.Updated<LegalPersonProfile>(entity.Id);

                _legalPersonProfileSecureRepository.Save();

                foreach (var entityInstanceDto in entityInstanceDtos)
                {
                    _updateDynamicAggregateRepository.Update(entityInstanceDto.EntityInstance, entityInstanceDto.PropertyInstances);
                }

                operationScope.Complete();
            }
        }
    }
}