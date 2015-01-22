using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.LegalPersons.Operations
{
    public class CreateLegalPersonProfileAggregateService : IAggregateRootRepository<LegalPerson>, ICreateAggregateRepository<LegalPersonProfile>
    {
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISecureRepository<LegalPersonProfile> _legalPersonProfileSecureRepository;

        public CreateLegalPersonProfileAggregateService(
            IIdentityProvider identityProvider,
            IOperationScopeFactory operationScopeFactory,
            ISecureRepository<LegalPersonProfile> legalPersonProfileSecureRepository)
        {
            _identityProvider = identityProvider;
            _operationScopeFactory = operationScopeFactory;
            _legalPersonProfileSecureRepository = legalPersonProfileSecureRepository;
        }

        public int Create(LegalPersonProfile entity)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, LegalPersonProfile>())
            {
                _identityProvider.SetFor(entity);
                _legalPersonProfileSecureRepository.Add(entity);
                operationScope.Added<LegalPersonProfile>(entity.Id);

                var count = _legalPersonProfileSecureRepository.Save();

                operationScope.Complete();

                return count;
            }
        }
    }
}