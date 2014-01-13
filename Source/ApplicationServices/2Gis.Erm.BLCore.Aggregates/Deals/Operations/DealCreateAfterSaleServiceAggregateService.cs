using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Deals.Operations
{
    public sealed class DealCreateAfterSaleServiceAggregateService : IDealCreateAfterSaleServiceAggregateService
    {
        private readonly IRepository<AfterSaleServiceActivity> _afterSaleServiceGenericRepository;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;

        public DealCreateAfterSaleServiceAggregateService(
            IRepository<AfterSaleServiceActivity> afterSaleServiceGenericRepository, 
            IIdentityProvider identityProvider,
            IOperationScopeFactory scopeFactory)
        {
            _afterSaleServiceGenericRepository = afterSaleServiceGenericRepository;
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
        }

        public void Create(AfterSaleServiceActivity activity)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, AfterSaleServiceActivity>())
            {
                _identityProvider.SetFor(activity);
                _afterSaleServiceGenericRepository.Add(activity);
                _afterSaleServiceGenericRepository.Save();

                scope.Added<AfterSaleServiceActivity>(activity.Id)
                     .Complete();
            }
        }
    }
}