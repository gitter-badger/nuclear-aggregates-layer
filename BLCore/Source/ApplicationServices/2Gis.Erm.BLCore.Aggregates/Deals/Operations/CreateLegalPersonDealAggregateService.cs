using DoubleGis.Erm.BLCore.API.Aggregates.Deals.Operations;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage.Writings;

namespace DoubleGis.Erm.BLCore.Aggregates.Deals.Operations
{
    public class CreateLegalPersonDealAggregateService : ICreateLegalPersonDealAggregateService
    {
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<LegalPersonDeal> _legalPersonDealLinkRepository;

        public CreateLegalPersonDealAggregateService(
            IIdentityProvider identityProvider,
            IOperationScopeFactory operationScopeFactory,
            IRepository<LegalPersonDeal> legalPersonDealLinkRepository)
        {
            _identityProvider = identityProvider;
            _operationScopeFactory = operationScopeFactory;
            _legalPersonDealLinkRepository = legalPersonDealLinkRepository;
        }

        public void Create(LegalPersonDeal legalPersonDeal)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, LegalPersonDeal>())
            {
                _identityProvider.SetFor(legalPersonDeal);
                _legalPersonDealLinkRepository.Add(legalPersonDeal);
                operationScope.Added(legalPersonDeal);

                _legalPersonDealLinkRepository.Save();

                operationScope.Complete();
            }
        }
    }
}