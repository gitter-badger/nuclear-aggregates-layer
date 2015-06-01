using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage.Writings;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices.Operations
{
    public class ActivatePriceAggregateService : IActivatePriceAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<Price> _priceGenericRepository;

        public ActivatePriceAggregateService(IOperationScopeFactory operationScopeFactory,
                                             IRepository<Price> priceGenericRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _priceGenericRepository = priceGenericRepository;
        }

        public int Activate(Price price)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<ActivateIdentity, Price>())
            {
                price.IsActive = true;
                price.IsPublished = false;
                
                _priceGenericRepository.Update(price);
                operationScope.Updated<Price>(price.Id);

                var count = _priceGenericRepository.Save();

                operationScope.Complete();
                return count;
            }
        }
    }
}