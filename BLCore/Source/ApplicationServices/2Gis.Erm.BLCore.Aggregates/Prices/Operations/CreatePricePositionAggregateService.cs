using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using NuClear.Storage;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices.Operations
{
    public class CreatePricePositionAggregateService : ICreatePricePositionAggregateService
    {
        private readonly IIdentityProvider _identityProvider;
        private readonly IRepository<PricePosition> _pricePositionGenericRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public CreatePricePositionAggregateService(IIdentityProvider identityProvider,
                                                   IRepository<PricePosition> pricePositionGenericRepository,
                                                   IOperationScopeFactory operationScopeFactory)
        {
            _identityProvider = identityProvider;
            _pricePositionGenericRepository = pricePositionGenericRepository;
            _operationScopeFactory = operationScopeFactory;
        }

        public void Create(PricePosition pricePosition)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, PricePosition>())
            {
                _identityProvider.SetFor(pricePosition);

                _pricePositionGenericRepository.Add(pricePosition);
                _pricePositionGenericRepository.Save();

                operationScope.Added(pricePosition)
                              .Complete();
            }
        }
    }
}