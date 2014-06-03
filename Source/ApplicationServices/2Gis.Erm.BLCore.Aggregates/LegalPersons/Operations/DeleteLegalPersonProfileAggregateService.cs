using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.LegalPersons.Operations
{
    public class DeleteLegalPersonProfileAggregateService : IAggregateRootRepository<LegalPerson>, IDeleteAggregateRepository<LegalPersonProfile>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISecureRepository<LegalPersonProfile> _legalPersonProfileSecureRepository;
        private readonly ILegalPersonReadModel _legalPersonReadModel;

        public DeleteLegalPersonProfileAggregateService(
            IOperationScopeFactory operationScopeFactory,
            ISecureRepository<LegalPersonProfile> legalPersonProfileSecureRepository,
            ILegalPersonReadModel legalPersonReadModel)
        {
            _operationScopeFactory = operationScopeFactory;
            _legalPersonProfileSecureRepository = legalPersonProfileSecureRepository;
            _legalPersonReadModel = legalPersonReadModel;
        }

        public int Delete(LegalPersonProfile entity)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, LegalPersonProfile>())
            {
                // Профиль юр. лица при удалении не удаляется, а... wait for it... деактивируется: https://confluence.2gis.ru/pages/viewpage.action?pageId=93160525
                entity.IsActive = false;
                _legalPersonProfileSecureRepository.Update(entity);
                operationScope.Updated<LegalPersonProfile>(entity.Id);

                var count = _legalPersonProfileSecureRepository.Save();

                operationScope.Complete();

                return count;
            }
            }

        public int Delete(long entityId)
        {
            return Delete(_legalPersonReadModel.GetLegalPersonProfile(entityId));
        }
    }
}