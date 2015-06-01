using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Storage.Writings;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.Operations
{
    public sealed class OrderCreateReleaseTotalsAggregateService : IOrderCreateReleaseTotalsAggregateService
    {
        private readonly IRepository<OrderReleaseTotal> _orderReleaseTotalsRepository;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;

        public OrderCreateReleaseTotalsAggregateService(
            IRepository<OrderReleaseTotal> orderReleaseTotalsRepository, 
            IIdentityProvider identityProvider,
            IOperationScopeFactory scopeFactory)
        {
            _orderReleaseTotalsRepository = orderReleaseTotalsRepository;
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
        }

        public void Create(IEnumerable<OrderReleaseTotal> releaseTotals)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, OrderReleaseTotal>())
            {
                foreach (var releaseTotal in releaseTotals)
                {
                    _identityProvider.SetFor(releaseTotal);
                    _orderReleaseTotalsRepository.Add(releaseTotal);
                    scope.Added<OrderReleaseTotal>(releaseTotal.Id);
                }

                _orderReleaseTotalsRepository.Save();
                scope.Complete();
            }
        }
    }
}