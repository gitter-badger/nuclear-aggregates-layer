using System;
using System.Linq;
using System.Threading;
using System.Transactions;

using DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Releasing.Release
{
    public sealed class ServiceBusEnsureOrderExportedStrategy : IEnsureOrderExportedStrategy
    {
        private readonly IIntegrationSettings _integrationSettings;
        private readonly IOrderReadModel _orderReadModel;
        private readonly IExportableOperationsPersistenceService<Order, ExportFlowOrdersOrder> _exportableOperationsPersistenceService;
        private readonly IExportRepository<Order> _exportRepository;
        private readonly ICommonLog _logger;

        public ServiceBusEnsureOrderExportedStrategy(IIntegrationSettings integrationSettings,
                                                     IOrderReadModel orderReadModel,
                                                     IExportableOperationsPersistenceService<Order, ExportFlowOrdersOrder> exportableOperationsPersistenceService,
                                                     IExportRepository<Order> exportRepository,
                                                     ICommonLog logger)
        {
            _integrationSettings = integrationSettings;
            _orderReadModel = orderReadModel;
            _exportableOperationsPersistenceService = exportableOperationsPersistenceService;
            _exportRepository = exportRepository;
            _logger = logger;
        }

        public bool IsExported(long releaseId, long organizationUnitId, int organizationUnitDgppId, TimePeriod period, bool isBeta)
        {
            if (!_integrationSettings.EnableIntegration)
            {
                throw new NotificationException("Interaction with service bus is disabled in Erm config settings. " +
                                                "Can't start ensure process that all orders are exported.");
            }

            _logger.InfoFormatEx("Starting ensure process that all orders are exported already to servicebus. " +
                                 "Release detail: id {0}, organization unit id {1}, period {2}, {3} release",
                                 releaseId,
                                 organizationUnitId,
                                 period,
                                 isBeta ? "beta" : "final");
            if (isBeta)
            {
                _logger.InfoFormatEx("Release type is beta, so check process is skipped. Release detail: id {0}, organization unit id {1}, period {2}",
                                     releaseId,
                                     organizationUnitId,
                                     period);
                return true;
            }

            // Если есть невыгруженные изменения, нужно дождаться, чтобы выполнилась задача экспорта заказов
            const int WaitTimeoutSec = 5;
            const int MaxAttempsCount = 5;
            int attempsCount = 1;

            while (!IsRequiredOrdersAreExported(organizationUnitId, period))
            {
                if (attempsCount == MaxAttempsCount)
                {
                    _logger.FatalFormatEx("Aborted ensure process that all orders are exported already to servicebus. " +
                                          "Max attempts count achieved {0}. Release detail: id {1}, organization unit id {2}, period {3}, final release",
                                          MaxAttempsCount,
                                          releaseId,
                                          organizationUnitId,
                                          period);

                    return false;
                }

                _logger.WarnFormatEx("Ensure process that all orders are exported already to servicebus. " +
                                     "Waiting orders async export activity completed (Running by 2GIS ERM Asynchronous Processing Service). " +
                                     "Release id: {0}. Current attempt number: {1}. Periodic check interval in sec: {2}",
                                     releaseId,
                                     attempsCount,
                                     WaitTimeoutSec);

                Thread.Sleep(WaitTimeoutSec * 1000);
                ++attempsCount;
            }

            _logger.InfoFormatEx("Finished ensure process. Ensured that all orders are exported already to servicebus. " +
                                 "Release detail: id {0}, organization unit id {1}, period {2}, final release",
                                 releaseId,
                                 organizationUnitId,
                                 period);
            return true;
        }

        private bool IsRequiredOrdersAreExported(long organizationUnitId, TimePeriod period)
        {
            bool isExported;
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                isExported = AllOrdersAreSuccessfullyExported(organizationUnitId, period) && AllOperationsAreProcessed(organizationUnitId, period);
                transaction.Complete();
            }

            return isExported;
        }

        private bool AllOrdersAreSuccessfullyExported(long organizationUnitId, TimePeriod period)
        {
            var failedOrders = _exportableOperationsPersistenceService.GetFailedEntities(int.MaxValue, 0).Select(entity => entity.EntityId);
            var anyFailedOrder = _orderReadModel.GetOrdersForRelease(organizationUnitId, period).Any(order => failedOrders.Contains(order.Id));
            return !anyFailedOrder;
        }

        private bool AllOperationsAreProcessed(long organizationUnitId, TimePeriod period)
        {
            var selectSpecification = new SelectSpecification<Order, long>(order => order.Id);
            var oneYearOperationsInterval = DateTime.UtcNow.AddYears(-1);
            var operations = _exportableOperationsPersistenceService.GetPendingOperations(oneYearOperationsInterval, int.MaxValue);
            var query = _exportRepository.GetBuilderForOperations(operations);
            var orders = _exportRepository.GetEntityDtos(query,
                                                         selectSpecification,
                                                         OrderSpecs.Orders.Find.ForRelease(organizationUnitId, period));

            return !orders.Any();
        }
    }
}