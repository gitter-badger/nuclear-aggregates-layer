using DoubleGis.Erm.BLCore.API.Aggregates.Deals.Operations;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Deals.Operations
{
    public class CreateDealAggregateService : ICreateDealAggregateService
    {
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ISecureRepository<Deal> _dealSecureRepository;

        public CreateDealAggregateService(
            IIdentityProvider identityProvider,
            IOperationScopeFactory operationScopeFactory,
            ISecureRepository<Deal> dealSecureRepository)
        {
            _identityProvider = identityProvider;
            _operationScopeFactory = operationScopeFactory;
            _dealSecureRepository = dealSecureRepository;
        }

        public void Create(Deal deal)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, Deal>())
            {
                _identityProvider.SetFor(deal);

                _dealSecureRepository.Add(deal);
                _dealSecureRepository.Save();

                operationScope.Added(deal).Complete();
            }
        }
    }
}
