using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Releasing.Release
{
    public sealed class ServiceBusEnsureOrderExportedStrategy : IEnsureOrderExportedStrategy
    {
        private const int EntitiesAmountLogLimiter = 100;
        private readonly IIntegrationSettings _integrationSettings;
        private readonly IOrderReadModel _orderReadModel;
        private readonly IOperationsProcessingsStoreService<Order, ExportFlowOrdersOrder> _orderOperationsProcessingsStoreService;
        private readonly IOperationsProcessingsStoreService<Order, ExportFlowOrdersInvoice> _invoiceOperationsProcessingsStoreService;
        private readonly IExportRepository<Order> _exportRepository;
        private readonly ITracer _logger;

        public ServiceBusEnsureOrderExportedStrategy(IIntegrationSettings integrationSettings,
                                                     IOrderReadModel orderReadModel,
                                                     IOperationsProcessingsStoreService<Order, ExportFlowOrdersOrder> operationsProcessingsStoreService,
                                                     IExportRepository<Order> exportRepository,
                                                     ITracer logger,
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
                isExported = FailedToExportOrdersQueueIsEmpty(organizationUnitId, period);
                if (isExported)
                {
                    var ordersToProcess = EntitiesToProcess(organizationUnitId,
                                                            period,
                                                            _orderOperationsProcessingsStoreService);
                    if (ordersToProcess.Any())
                    {
                        _logger.WarnFormat("Not all required orders are exported. The queue to process contains following orders: {0}",
                                           string.Join(",", ordersToProcess.Take(EntitiesAmountLogLimiter).Select(x => x.ToString())));
                    }

                    isExported &= !ordersToProcess.Any();
                }

                if (isExported)
                {
                    var invoicesToProcess = EntitiesToProcess(organizationUnitId,
                                                              period,
                                                              _invoiceOperationsProcessingsStoreService);
                    if (invoicesToProcess.Any())
                    {
                        _logger.WarnFormat("Not all required invoices are exported. The queue to process contains following invoices: {0}",
                                           string.Join(",", invoicesToProcess.Take(EntitiesAmountLogLimiter).Select(x => x.ToString())));
                    }

                    isExported &= !invoicesToProcess.Any();
                }

                transaction.Complete();
            }

            return isExported;
        }

        private bool FailedToExportOrdersQueueIsEmpty(long organizationUnitId, TimePeriod period)
        {
            var failedOrders = _orderOperationsProcessingsStoreService.GetFailedEntities().Select(entity => entity.EntityId);
            var failedInvoices = _invoiceOperationsProcessingsStoreService.GetFailedEntities().Select(entity => entity.EntityId);
            var ordersForRelease = _orderReadModel.GetOrderIdsForRelease(organizationUnitId, period);

            var failedOrdersForRelease = ordersForRelease.Where(failedOrders.Contains).ToArray();
            var failedInvoicesForRelease = ordersForRelease.Where(failedInvoices.Contains).ToArray();
            if (failedOrdersForRelease.Any())
            {
                _logger.WarnFormat("Not all required orders are exported. The failed entities queue contains following orders: {0}",
                                     string.Join(",", failedOrdersForRelease.Take(EntitiesAmountLogLimiter).Select(x => x.ToString())));
            }

            if (failedInvoicesForRelease.Any())
            {
                _logger.WarnFormat("Not all required invoices are exported. The failed entities queue contains following invoices: {0}",
                                     string.Join(",", failedInvoicesForRelease.Take(EntitiesAmountLogLimiter).Select(x => x.ToString())));
            }

            return !failedOrdersForRelease.Any() && !failedInvoicesForRelease.Any();
        }

        private IEnumerable<long> EntitiesToProcess(long organizationUnitId,
                                                    TimePeriod period,
                                                    IOperationsProcessingsStoreService operationsProcessingsStoreService)
        {
            var selectSpecification = new SelectSpecification<Order, long>(order => order.Id);
            var oneMonthOperationsInterval = DateTime.UtcNow.AddMonths(-1);
            var operations = operationsProcessingsStoreService.GetPendingOperations(oneMonthOperationsInterval);
            var query = _exportRepository.GetBuilderForOperations(operations);
            return _exportRepository.GetEntityDtos(query,
                                                   selectSpecification,
                                                   OrderSpecs.Orders.Find.ForRelease(organizationUnitId, period));
        }
    }
}