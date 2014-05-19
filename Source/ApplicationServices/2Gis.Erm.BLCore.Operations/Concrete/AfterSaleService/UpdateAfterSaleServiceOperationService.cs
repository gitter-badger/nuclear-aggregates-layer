using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Deals.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.AfterSaleService;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AfterSaleService;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.AfterSaleService
{
    public sealed class UpdateAfterSaleServiceOperationService : IUpdateAfterSaleServiceOperationService
    {
        private readonly IDealReadModel _dealReadModel;
        private readonly IDeleteAfterSaleServiceAggregateService _deleteAfterSaleServiceAggregateService;
        private readonly IOperationScopeFactory _scopeFactory;

        public UpdateAfterSaleServiceOperationService(
            IDealReadModel dealReadModel,
            IDeleteAfterSaleServiceAggregateService deleteAfterSaleServiceAggregateService,
            IOperationScopeFactory scopeFactory)
        {
            _dealReadModel = dealReadModel;
            _deleteAfterSaleServiceAggregateService = deleteAfterSaleServiceAggregateService;
            _scopeFactory = scopeFactory;
        }

        public void Update(Guid dealReplicationCode, DateTime activityDate, AfterSaleServiceType afterSaleServiceType)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<UpdateAfterSaleServiceIdentity>())
            {
                var existingAftersaleService = _dealReadModel.GetAfterSaleService(dealReplicationCode, activityDate, afterSaleServiceType);
                if (existingAftersaleService != null)
                {
                    _deleteAfterSaleServiceAggregateService.Delete(existingAftersaleService);
                }

                scope.Complete();
            }
        }
    }
}