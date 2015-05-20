using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices.Operations
{
    public class DeletePriceAggregateService : IDeletePriceAggregateService
    {
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IRepository<Price> _priceGenericRepository;

        public DeletePriceAggregateService(IOperationScopeFactory operationScopeFactory,
                                           IRepository<Price> priceGenericRepository)
        {
            _operationScopeFactory = operationScopeFactory;
            _priceGenericRepository = priceGenericRepository;
        }

        public int Delete(Price price)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<DeleteIdentity, Price>())
            {
                _priceGenericRepository.Delete(price);
                operationScope.Deleted<Price>(price.Id);

                var count = _priceGenericRepository.Save();

                operationScope.Complete();
                return count;
            }
        }
    }
}