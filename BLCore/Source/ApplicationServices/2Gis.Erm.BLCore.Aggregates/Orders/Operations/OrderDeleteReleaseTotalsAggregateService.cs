using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage.Writings;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.Operations
{
    public sealed class OrderDeleteReleaseTotalsAggregateService : IOrderDeleteReleaseTotalsAggregateService
    {
        private readonly IRepository<OrderReleaseTotal> _orderReleaseTotalsRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public OrderDeleteReleaseTotalsAggregateService(IRepository<OrderReleaseTotal> orderReleaseTotalsRepository, IOperationScopeFactory scopeFactory)
        {
            _orderReleaseTotalsRepository = orderReleaseTotalsRepository;
            _scopeFactory = scopeFactory;
        }

        public void Delete(IEnumerable<OrderReleaseTotal> releaseTotals)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, OrderReleaseTotal>())
            {
                foreach (var releaseTotal in releaseTotals)
                {
                    _orderReleaseTotalsRepository.Delete(releaseTotal);
                    scope.Deleted<OrderReleaseTotal>(releaseTotal.Id);
                }

                _orderReleaseTotalsRepository.Save();
                scope.Complete();
            }
        }
    }
}
