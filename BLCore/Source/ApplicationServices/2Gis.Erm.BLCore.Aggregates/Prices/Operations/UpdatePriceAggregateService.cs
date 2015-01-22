using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices.Operations
{
    public class UpdatePriceAggregateService : IAggregateRootRepository<Price>, IUpdateAggregateRepository<Price>
    {
        private readonly IRepository<Price> _priceGenericRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public UpdatePriceAggregateService(IRepository<Price> priceGenericRepository, IOperationScopeFactory operationScopeFactory)
        {
            _priceGenericRepository = priceGenericRepository;
            _operationScopeFactory = operationScopeFactory;
        }

        public int Update(Price entity)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, Price>())
            {
                _priceGenericRepository.Update(entity);
                operationScope.Updated<Price>(entity.Id);

                var count = _priceGenericRepository.Save();

                operationScope.Complete();
                return count;
            }
        }
    }
}