using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage.Writings;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices.Operations
{
    public class CreatePriceAggregateService : ICreatePriceAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IIdentityProvider _identityProvider;
        private readonly IRepository<Price> _priceGenericRepository;

        public CreatePriceAggregateService(IOperationScopeFactory operationScopeFactory,
                                           IIdentityProvider identityProvider,
                                           IRepository<Price> priceGenericRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _identityProvider = identityProvider;
            _priceGenericRepository = priceGenericRepository;
        }

        public int Create(Price price, long currencyId)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, Price>())
            {
                _identityProvider.SetFor(price);
                price.CurrencyId = currencyId;

                _priceGenericRepository.Add(price);
                operationScope.Added<Price>(price.Id);

                var count = _priceGenericRepository.Save();

                operationScope.Complete();
                return count;
            }
        }
    }
}