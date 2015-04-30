using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using NuClear.Storage;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices.Operations
{
    public class DeactivatePriceAggregateService : IDeactivatePriceAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<Price> _priceGenericRepository;

        public DeactivatePriceAggregateService(IOperationScopeFactory operationScopeFactory,
                                               IRepository<Price> priceGenericRepository)

        {
            _operationScopeFactory = operationScopeFactory;
            _priceGenericRepository = priceGenericRepository;
        }

        public int Deactivate(Price price)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeactivateIdentity, Price>())
            {
                price.IsActive = false;
                _priceGenericRepository.Update(price);
                operationScope.Updated<Price>(price.Id);

                var count = _priceGenericRepository.Save();

                operationScope.Complete();
                return count;
            }
        }
    }
}