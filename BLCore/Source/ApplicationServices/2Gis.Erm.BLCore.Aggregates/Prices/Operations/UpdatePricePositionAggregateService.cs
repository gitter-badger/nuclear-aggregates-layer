using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage.Writings;

namespace DoubleGis.Erm.BLCore.Aggregates.Prices.Operations
{
    public class UpdatePricePositionAggregateService : IAggregateRootService<Price>, IUpdateAggregateRepository<PricePosition>
    {
        private readonly IRepository<PricePosition> _pricePositionGenericRepository;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public UpdatePricePositionAggregateService(IRepository<PricePosition> pricePositionGenericRepository, IOperationScopeFactory operationScopeFactory)
        {
            _pricePositionGenericRepository = pricePositionGenericRepository;
            _operationScopeFactory = operationScopeFactory;
        }

        public int Update(PricePosition entity)
        {
            using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, PricePosition>())
            {
                _pricePositionGenericRepository.Update(entity);
                operationScope.Updated<PricePosition>(entity.Id);

                var count = _pricePositionGenericRepository.Save();

                operationScope.Complete();
                return count;
            }
        }
    }
}