using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Deals.Operations
{
    public sealed class DeleteAfterSaleServiceAggregateService : IDeleteAfterSaleServiceAggregateService
    {
        private readonly IRepository<AfterSaleServiceActivity> _afterSaleServiceGenericRepository;
        private readonly IOperationScopeFactory _scopeFactory;

        public DeleteAfterSaleServiceAggregateService(
            IRepository<AfterSaleServiceActivity> afterSaleServiceGenericRepository, 
            IOperationScopeFactory scopeFactory)
        {
            _afterSaleServiceGenericRepository = afterSaleServiceGenericRepository;
            _scopeFactory = scopeFactory;
        }

        public void Delete(AfterSaleServiceActivity activity)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, AfterSaleServiceActivity>())
            {
                _afterSaleServiceGenericRepository.Delete(activity);
                _afterSaleServiceGenericRepository.Save();

                scope.Deleted<AfterSaleServiceActivity>(activity.Id)
                     .Complete();
            }
        }
    }
}