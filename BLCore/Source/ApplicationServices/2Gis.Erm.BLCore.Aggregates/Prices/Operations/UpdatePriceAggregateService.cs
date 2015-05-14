using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices.Operations
{
    public class UpdatePriceAggregateService : IUpdatePriceAggregateService
    {
        private readonly IRepository<Price> _priceGenericRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public UpdatePriceAggregateService(IRepository<Price> priceGenericRepository, IOperationScopeFactory operationScopeFactory)
        {
            _priceGenericRepository = priceGenericRepository;
            _operationScopeFactory = operationScopeFactory;
        }

        public void Update(Price price)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, Price>())
            {
                _priceGenericRepository.Update(price);
                _priceGenericRepository.Save();

                operationScope.Updated(price)
                              .Complete();
            }
        }
    }
}