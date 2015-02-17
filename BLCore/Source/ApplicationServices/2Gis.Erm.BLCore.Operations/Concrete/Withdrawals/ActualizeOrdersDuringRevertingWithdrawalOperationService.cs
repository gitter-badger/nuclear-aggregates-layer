using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals;
using DoubleGis.Erm.Platform.API.Core.ActionLogging;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Withdrawals
{
    public sealed class ActualizeOrdersDuringRevertingWithdrawalOperationService : IActualizeOrdersDuringRevertingWithdrawalOperationService
    {
        private readonly IOrderActualizeOrdersAmoutDuringWithdrawalAggregateService _orderActualizeOrdersAmoutDuringWithdrawalAggregateService;
        private readonly IOrderRestoreOrdersFromArchiveAggregateService _restoreOrdersFromArchiveAggregateService;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IActionLogger _actionLogger;
        private readonly ICommonLog _logger;

        public ActualizeOrdersDuringRevertingWithdrawalOperationService(
            IOrderActualizeOrdersAmoutDuringWithdrawalAggregateService orderActualizeOrdersAmoutDuringWithdrawalAggregateService,
            IOrderRestoreOrdersFromArchiveAggregateService restoreOrdersFromArchiveAggregateService,
            IOperationScopeFactory scopeFactory,
            IActionLogger actionLogger,
            ICommonLog logger)
        {
            _orderActualizeOrdersAmoutDuringWithdrawalAggregateService = orderActualizeOrdersAmoutDuringWithdrawalAggregateService;
            _restoreOrdersFromArchiveAggregateService = restoreOrdersFromArchiveAggregateService;
            _scopeFactory = scopeFactory;
            _actionLogger = actionLogger;
            _logger = logger;
        }

        public void Actualize(IEnumerable<ActualizeOrdersDto> orderInfos)
        {
            _logger.Info("Starting actualize orders during reverting withdrawal process");

            using (var scope = _scopeFactory.CreateNonCoupled<ActualizeOrdersDuringRevertingWithdrawalIdentity>())
            {
                _orderActualizeOrdersAmoutDuringWithdrawalAggregateService.Actualize(orderInfos, true);
                var changesForRestoredOrders = _restoreOrdersFromArchiveAggregateService.Restore(orderInfos.Select(info => info.Order));
                _actionLogger.LogChanges(changesForRestoredOrders);

                scope.Complete();
            }

            _logger.Info("Finished actualize orders during reverting withdrawal process");
        }
    }
}