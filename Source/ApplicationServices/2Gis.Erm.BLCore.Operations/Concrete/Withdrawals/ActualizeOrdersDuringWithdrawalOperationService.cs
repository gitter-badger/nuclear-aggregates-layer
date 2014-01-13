﻿using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Orders.Operations;
using DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Withdrawals
{
    public sealed class ActualizeOrdersDuringWithdrawalOperationService : IActualizeOrdersDuringWithdrawalOperationService
    {
        private readonly IOrderReadModel _orderReadModel;
        private readonly IOrderActualizeOrdersAmoutDuringWithdrawalAggregateService _orderActualizeOrdersAmoutDuringWithdrawalAggregateService;
        private readonly IOrderChangeStateOrders2ArchiveAggregateService _orderChangeStateOrders2ArchiveAggregateService;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IActionLogger _actionLogger;
        private readonly ICommonLog _logger;

        public ActualizeOrdersDuringWithdrawalOperationService(
            IOrderReadModel orderReadModel,
            IOrderActualizeOrdersAmoutDuringWithdrawalAggregateService orderActualizeOrdersAmoutDuringWithdrawalAggregateService,
            IOrderChangeStateOrders2ArchiveAggregateService orderChangeStateOrders2ArchiveAggregateService,
            IOperationScopeFactory scopeFactory,
            IActionLogger actionLogger,
            ICommonLog logger)
        {
            _orderReadModel = orderReadModel;
            _orderActualizeOrdersAmoutDuringWithdrawalAggregateService = orderActualizeOrdersAmoutDuringWithdrawalAggregateService;
            _orderChangeStateOrders2ArchiveAggregateService = orderChangeStateOrders2ArchiveAggregateService;
            _scopeFactory = scopeFactory;
            _actionLogger = actionLogger;
            _logger = logger;
        }

        public void Actualize(long withdrawalOrganizationUnitId, IEnumerable<ActualizeOrdersDto> orderInfos)
        {
            _logger.InfoEx("Starting actialize orders during withdrawal process");

            using (var scope = _scopeFactory.CreateNonCoupled<ActualizeOrdersDuringWithdrawalIdentity>())
            {
                var actualizedOrdersMap = _orderActualizeOrdersAmoutDuringWithdrawalAggregateService.Actualize(orderInfos, false);

                // переводим в архив как заказы, которые стали отразмещавшимися после выполнения списания (погашения блокировок)
                // так и старые заказы в статусе "на расторжении" тоже переводим в архив, они все попадут в одну и ту же выборку
                var ordersForArchive = 
                    _orderReadModel
                        .GetOrdersCompletelyReleasedBySourceOrganizationUnit(withdrawalOrganizationUnitId)
                        .Where(o => actualizedOrdersMap.ContainsKey(o.Id) || o.WorkflowStepId == (int)OrderState.OnTermination);
                var changesForArchivied = _orderChangeStateOrders2ArchiveAggregateService.Archiving(ordersForArchive);
                _actionLogger.LogChanges(changesForArchivied);

                scope.Complete();
            }

            _logger.InfoEx("Finished actialize orders during withdrawal process");
        }
    }
}