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

        public int Create(PricePosition pricePosition, long priceId, long positionId)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, PricePosition>())
            {
                _identityProvider.SetFor(pricePosition);
                pricePosition.PriceId = priceId;
                pricePosition.PositionId = positionId;

                _pricePositionGenericRepository.Add(pricePosition);
                operationScope.Added<PricePosition>(pricePosition.Id);

                var count = _pricePositionGenericRepository.Save();

                operationScope.Complete();

                return count;
            }
        }
    }
}