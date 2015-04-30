using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.LegalPersons.Operations
{
    public class CreateLegalPersonAggregateService : IAggregateRootRepository<LegalPerson>, ICreateAggregateRepository<LegalPerson>
    {
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISecureRepository<LegalPerson> _legalPersonSecureRepository;

        public CreateLegalPersonAggregateService(
            IIdentityProvider identityProvider,
            IOperationScopeFactory operationScopeFactory,
            ISecureRepository<LegalPerson> legalPersonSecureRepository)
        {
            _identityProvider = identityProvider;
            _operationScopeFactory = operationScopeFactory;
            _legalPersonSecureRepository = legalPersonSecureRepository;
        }

        public int Create(LegalPerson entity)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, LegalPerson>())
            {
                _identityProvider.SetFor(entity);
                _legalPersonSecureRepository.Add(entity);
                operationScope.Added<LegalPerson>(entity.Id);
                
                var count = _legalPersonSecureRepository.Save();
                
                operationScope.Complete();

                return count;
            }
        }
    }
}