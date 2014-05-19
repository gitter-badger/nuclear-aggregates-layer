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
    public class UpdateLegalPersonAggregateService : IUpdatePartableEntityAggregateService<LegalPerson, LegalPerson>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISecureRepository<LegalPerson> _legalPersonSecureRepository;
        private readonly IUpdateDynamicAggregateRepository<BusinessEntityInstance, BusinessEntityPropertyInstance> _updateDynamicAggregateRepository;

        public UpdateLegalPersonAggregateService(
            IOperationScopeFactory operationScopeFactory,
            ISecureRepository<LegalPerson> legalPersonSecureRepository,
            IUpdateDynamicAggregateRepository<BusinessEntityInstance, BusinessEntityPropertyInstance> updateDynamicAggregateRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _legalPersonSecureRepository = legalPersonSecureRepository;
            _updateDynamicAggregateRepository = updateDynamicAggregateRepository;
        }

        public void Update(LegalPerson entity, IEnumerable<BusinessEntityInstanceDto> entityInstanceDtos)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, LegalPerson>())
            {
                _legalPersonSecureRepository.Update(entity);
                operationScope.Updated<LegalPerson>(entity.Id);

                _legalPersonSecureRepository.Save();

                foreach (var entityInstanceDto in entityInstanceDtos)
                {
                    _updateDynamicAggregateRepository.Update(entityInstanceDto.EntityInstance, entityInstanceDto.PropertyInstances);
                }

                operationScope.Complete();
            }
        }
    }
}