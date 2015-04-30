using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.LegalPersons.Operations
{
    public class DeleteLegalPersonAggregateService : IDeleteLegalPersonAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISecureRepository<LegalPerson> _legalPersonSecureRepository;
        private readonly ISecureRepository<LegalPersonProfile> _legalPersonProfileSecureRepository;

        public DeleteLegalPersonAggregateService(
            IOperationScopeFactory operationScopeFactory,
            ISecureRepository<LegalPerson> legalPersonSecureRepository,
            ISecureRepository<LegalPersonProfile> legalPersonProfileSecureRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _legalPersonSecureRepository = legalPersonSecureRepository;
            _legalPersonProfileSecureRepository = legalPersonProfileSecureRepository;
        }

        public void Delete(LegalPerson legalPerson, IEnumerable<LegalPersonProfile> profiles)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, LegalPerson>())
            {
                foreach (var profile in profiles)
                {
                    _legalPersonProfileSecureRepository.Delete(profile);
                    operationScope.Deleted(profile);
                }

                _legalPersonProfileSecureRepository.Save();

                _legalPersonSecureRepository.Delete(legalPerson);
                _legalPersonSecureRepository.Save();

                operationScope.Deleted(legalPerson);
                operationScope.Complete();
            }
        }
    }
}