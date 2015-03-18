﻿using DoubleGis.Erm.BLCore.API.Aggregates.Deals.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Deals.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Deals;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Deal;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Deals
{
    public sealed class ChangeDealStageOperationService : IChangeDealStageOperationService
    {
        private readonly IDealReadModel _dealReadModel;
        private readonly IDealChangeStageAggregateService _dealChangeStageAggregateService;
        private readonly IActionLogger _actionLogger;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly ITracer _tracer;

        public ChangeDealStageOperationService(
            IDealReadModel dealReadModel,
            IDealChangeStageAggregateService dealChangeStageAggregateService,
            IActionLogger actionLogger,
            IOperationScopeFactory scopeFactory, 
            ITracer tracer)
        {
            _dealReadModel = dealReadModel;
            _dealChangeStageAggregateService = dealChangeStageAggregateService;
            _actionLogger = actionLogger;
            _scopeFactory = scopeFactory;
            _tracer = tracer;
        }

        public void Change(long dealId, DealStage dealStage)
        {
            _tracer.DebugFormat("Deal replication. DealId: {0}, DealStage: {1}", dealId, dealStage);

            var deal = _dealReadModel.GetDeal(dealId);
            if (deal.DealStage == dealStage) return;

            if (IsPreparationStage(dealStage))
            {
                // сделка не может вернуться на подготовительный этап, если по ней уже начата деятельность или у нее есть активные заказы
                if (IsInProcessStage(dealStage) || _dealReadModel.HasOrders(dealId))
                {
                    _tracer.DebugFormat("Deal stage cannot be changed from {0} to {1}.", dealStage, deal.DealStage);
                    return;
                }
            }

            using (var scope = _scopeFactory.CreateNonCoupled<ChangeDealStageIdentity>())
            {
                var changes = _dealChangeStageAggregateService.ChangeStage(new[] { new DealChangeStageDto { Deal = deal, NextStage = dealStage } });
                _actionLogger.LogChanges(changes);

                scope.Updated<Deal>(deal.Id);
                scope.Complete();
            }
        }

        private static bool IsPreparationStage(DealStage dealStage)
        {
            return dealStage == DealStage.CollectInformation 
                || dealStage == DealStage.HoldingProductPresentation 
                || dealStage == DealStage.MatchAndSendProposition;
        }

        private static bool IsInProcessStage(DealStage dealStage)
        {
            return dealStage == DealStage.OrderApprovedForRelease 
                || dealStage == DealStage.OrderFormed
                || dealStage == DealStage.Service;
        }
    }
}