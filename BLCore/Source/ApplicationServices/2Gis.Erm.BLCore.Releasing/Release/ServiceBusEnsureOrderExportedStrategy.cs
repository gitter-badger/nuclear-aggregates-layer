using System;
using System.Linq;
using System.Threading;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
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
        private readonly IOperationsProcessingsStoreService<Order, ExportFlowOrdersOrder> _orderOperationsProcessingsStoreService;
        private readonly IOperationsProcessingsStoreService<Order, ExportFlowOrdersInvoice> _invoiceOperationsProcessingsStoreService;
        private readonly IExportRepository<Order> _exportRepository;
        private readonly ICommonLog _logger;

        public ServiceBusEnsureOrderExportedStrategy(IIntegrationSettings integrationSettings,
                                                     IOrderReadModel orderReadModel,
                                                     IOperationsProcessingsStoreService<Order, ExportFlowOrdersOrder> operationsProcessingsStoreService,
                                                     IExportRepository<Order> exportRepository,
                                                     ICommonLog logger,
                                                     IOperationsProcessingsStoreService<Order, ExportFlowOrdersInvoice> invoiceOperationsProcessingsStoreService)
        {
            _integrationSettings = integrationSettings;
            _orderReadModel = orderReadModel;
            _orderOperationsProcessingsStoreService = operationsProcessingsStoreService;
            _exportRepository = exportRepository;
            _logger = logger;
            _invoiceOperationsProcessingsStoreService = invoiceOperationsProcessingsStoreService;
        }

        public bool IsExported(long releaseId, long organizationUnitId, int organizationUnitDgppId, TimePeriod period, bool isBeta)
        {
            if (!_integrationSettings.EnableIntegration)
            {
                throw new NotificationException("Interaction with service bus is disabled in Erm config settings. " +
                                                "Can't start ensure process that all orders are exported.");
            }

            _logger.InfoFormat("Starting ensure process that all orders are exported already to servicebus. " +
                                 "Release detail: id {0}, organization unit id {1}, period {2}, {3} release",
                                 releaseId,
                                 organizationUnitId,
                                 period,
                                 isBeta ? "beta" : "final");
            if (isBeta)
            {
                _logger.InfoFormat("Release type is beta, so check process is skipped. Release detail: id {0}, organization unit id {1}, period {2}",
                                     releaseId,
                                     organizationUnitId,
                                     period);
                return true;
            }

            // Если есть невыгруженные изменения, нужно дождаться, чтобы выполнилась задача экспорта заказов
            const int WaitTimeoutSec = 5;
            const int MaxAttempsCount = 10;
            int attempsCount = 1;

            while (!IsRequiredOrdersAreExported(organizationUnitId, period))
            {
                if (attempsCount == MaxAttempsCount)
                {
                    _logger.FatalFormat("Aborted ensure process that all orders are exported already to servicebus. " +
                                          "Max attempts count achieved {0}. Release detail: id {1}, organization unit id {2}, period {3}, final release",
                                          MaxAttempsCount,
                                          releaseId,
                                          organizationUnitId,
                                          period);

                    return false;
                }

                _logger.WarnFormat("Ensure process that all orders are exported already to servicebus. " +
                                     "Waiting orders async export activity completed (Running by 2GIS ERM Asynchronous Processing Service). " +
                                     "Release id: {0}. Current attempt number: {1}. Periodic check interval in sec: {2}",
                                     releaseId,
                                     attempsCount,
                                     WaitTimeoutSec);

                Thread.Sleep(WaitTimeoutSec * 1000);
                ++attempsCount;
            }

            _logger.InfoFormat("Finished ensure process. Ensured that all orders are exported already to servicebus. " +
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
                isExported = AllOrdersAreSuccessfullyExported(organizationUnitId, period) &&
                             AllOperationsAreProcessed(organizationUnitId, period, _orderOperationsProcessingsStoreService) &&
                             AllOperationsAreProcessed(organizationUnitId, period, _invoiceOperationsProcessingsStoreService);
                transaction.Complete();
            }

            return isExported;
        }

        private bool AllOrdersAreSuccessfullyExported(long organizationUnitId, TimePeriod period)
        {
            var failedOrders = _orderOperationsProcessingsStoreService.GetFailedEntities().Select(entity => entity.EntityId);
            var failedInvoices = _invoiceOperationsProcessingsStoreService.GetFailedEntities().Select(entity => entity.EntityId);
            var anyFailedOrder = _orderReadModel.GetOrdersForRelease(organizationUnitId, period).Any(order => failedOrders.Contains(order.Id)
                                                                                                              || failedInvoices.Contains(order.Id));
            return !anyFailedOrder;
        }

        private bool AllOperationsAreProcessed(long organizationUnitId, TimePeriod period, IOperationsProcessingsStoreService operationsProcessingsStoreService)
        {
            var selectSpecification = new SelectSpecification<Order, long>(order => order.Id);
            var oneMonthOperationsInterval = DateTime.UtcNow.AddMonths(-1);
            var operations = operationsProcessingsStoreService.GetPendingOperations(oneMonthOperationsInterval);
            var query = _exportRepository.GetBuilderForOperations(operations);
            var orders = _exportRepository.GetEntityDtos(query,
                                                         selectSpecification,
                                                         OrderSpecs.Orders.Find.ForRelease(organizationUnitId, period));

            return !orders.Any();
        }
    }
}