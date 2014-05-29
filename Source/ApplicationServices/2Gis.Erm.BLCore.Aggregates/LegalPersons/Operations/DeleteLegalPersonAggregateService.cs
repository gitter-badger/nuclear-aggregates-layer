using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.LegalPersons.Operations
{
    public class DeleteLegalPersonAggregateService : IAggregateRootRepository<LegalPerson>, IDeleteAggregateRepository<LegalPerson>
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISecureRepository<LegalPerson> _legalPersonSecureRepository;
        private readonly ILegalPersonReadModel _legalPersonReadModel;
        private readonly IDeleteAggregateRepository<LegalPersonProfile> _profileDeleteAggregateRepository;

        public DeleteLegalPersonAggregateService(
            IOperationScopeFactory operationScopeFactory,
            ISecureRepository<LegalPerson> legalPersonSecureRepository,
            ILegalPersonReadModel legalPersonReadModel,
            IDeleteAggregateRepository<LegalPersonProfile> profileDeleteAggregateRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _legalPersonSecureRepository = legalPersonSecureRepository;
            _legalPersonReadModel = legalPersonReadModel;
            _profileDeleteAggregateRepository = profileDeleteAggregateRepository;
        }

        public int Delete(LegalPerson entity)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, LegalPerson>())
            {
                var profiles = _legalPersonReadModel.GetProfilesByLegalPerson(entity.Id);
                foreach (var profile in profiles)
                {
                    _profileDeleteAggregateRepository.Delete(profile.Id);
                }

                _legalPersonSecureRepository.Delete(entity);
                var count = _legalPersonSecureRepository.Save();

                operationScope.Deleted<LegalPerson>(entity.Id);
                operationScope.Complete();
                return count;
            }
            }

        public int Delete(long entityId)
        {
            return Delete(_legalPersonReadModel.GetLegalPerson(entityId));
        }
    }
}