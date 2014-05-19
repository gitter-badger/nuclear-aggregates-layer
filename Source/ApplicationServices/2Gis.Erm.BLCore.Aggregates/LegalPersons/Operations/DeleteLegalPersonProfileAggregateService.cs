using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Dynamic.Operations;
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

        public DeleteLegalPersonProfileAggregateService(
            IOperationScopeFactory operationScopeFactory,
            ISecureRepository<LegalPersonProfile> legalPersonProfileSecureRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _legalPersonProfileSecureRepository = legalPersonProfileSecureRepository;
        }

        public void Delete(LegalPersonProfile entity, IEnumerable<BusinessEntityInstanceDto> entityInstanceDtos)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, LegalPersonProfile>())
            {
                // Профиль юр. лица при удалении не удаляется, а... wait for it... деактивируется: https://confluence.2gis.ru/pages/viewpage.action?pageId=93160525
                entity.IsActive = false;
                _legalPersonProfileSecureRepository.Update(entity);
                operationScope.Updated<LegalPersonProfile>(entity.Id);

                _legalPersonProfileSecureRepository.Save();

                operationScope.Complete();
            }
        }
    }
}