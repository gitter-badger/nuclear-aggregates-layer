using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.LegalPersons.Operations
{
    public class DeactivateLegalPersonAggregateService : IDeactivateLegalPersonAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISecureRepository<LegalPerson> _legalPersonSecureRepository;
        private readonly ISecureRepository<LegalPersonProfile> _legalPersonProfileSecureRepository;

        public DeactivateLegalPersonAggregateService(
            IOperationScopeFactory operationScopeFactory,
            ISecureRepository<LegalPerson> legalPersonSecureRepository,
            ISecureRepository<LegalPersonProfile> legalPersonProfileSecureRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _legalPersonSecureRepository = legalPersonSecureRepository;
            _legalPersonProfileSecureRepository = legalPersonProfileSecureRepository;
        }

        public void Deactivate(LegalPerson legalPerson, IEnumerable<LegalPersonProfile> profiles)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeactivateIdentity, LegalPerson>())
            {
                foreach (var profile in profiles)
                {
                    profile.IsActive = false;
                    _legalPersonProfileSecureRepository.Update(profile);
                    operationScope.Updated(profile);
                }

                _legalPersonProfileSecureRepository.Save();

                legalPerson.IsActive = false;
                _legalPersonSecureRepository.Update(legalPerson);
                _legalPersonSecureRepository.Save();

                operationScope.Updated(legalPerson);
                operationScope.Complete();
            }
        }
    }
}