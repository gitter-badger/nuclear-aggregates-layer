using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.LegalPersons.Operations
{
    public class DeleteLegalPersonProfileAggregateService : IDeleteLegalPersonProfileAggregateService
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

        public void Delete(LegalPersonProfile legalPersonProfile)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, LegalPersonProfile>())
            {
                _legalPersonProfileSecureRepository.Delete(legalPersonProfile);
                operationScope.Deleted(legalPersonProfile);
                _legalPersonProfileSecureRepository.Save();

                operationScope.Complete();
            }
        }
    }
}