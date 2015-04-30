using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Entities.Aspects.Integration;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Export.Processors
{
    // FIXME {all, 26.03.2014}: в данном OperationService явно нарушается SRP - фактически он должен обеспечивать работу workflow обработки происходящих операций: 
    // - сформировать список операций требующих обработки (с учетом контекста конкретного процессора - пока это diff между уже обработанными операциями и всеми выполненными)
    // - обработать операции (чаще всего означает - выгрузить затронутые поерациями сущности в целевые потоки интеграции)
    // - сохранить контекст процессора (успешно обработанные операции, failed entities и т.п.)
    public abstract class IntegrationProcessorOperationService<TEntity, TProcessedOperationEntity> : IGenericIntegrationProcessorOperationService<TEntity, TProcessedOperationEntity>
        where TEntity : class, IEntity, IEntityKey
        where TProcessedOperationEntity : class, IEntity, IEntityKey, IIntegrationProcessorState, new()
    {
        private static readonly TimeSpan SafetyTimeFactor = TimeSpan.FromDays(1);

        private readonly IOperationsProcessingsStoreService<TEntity, TProcessedOperationEntity> _processingsStoreService;
        private readonly IOperationsExporter<TEntity, TProcessedOperationEntity> _operationsExporter;

        protected IntegrationProcessorOperationService(
            IOperationsProcessingsStoreService<TEntity, TProcessedOperationEntity> processingsStoreService,
            IOperationsExporter<TEntity, TProcessedOperationEntity> operationsExporter)
        {
            _processingsStoreService = processingsStoreService;
            _operationsExporter = operationsExporter;
        }

        protected ISelectSpecification<TProcessedOperationEntity, DateTime> ProcessingDateSpecification
        {
            get { return new SelectSpecification<TProcessedOperationEntity, DateTime>(operation => operation.Date); }
        }

        public IEnumerable<PerformedBusinessOperation> GetPendingOperations(int maxOperationCount)
        {
            var lastProcessedOperationPerformDate = _processingsStoreService.GetLastProcessedOperationPerformDate(ProcessingDateSpecification);
            if (lastProcessedOperationPerformDate > DateTime.MinValue)
            {
                lastProcessedOperationPerformDate = lastProcessedOperationPerformDate - SafetyTimeFactor;
            }

            return _processingsStoreService.GetPendingOperations(lastProcessedOperationPerformDate, maxOperationCount);
        }

        public void ExportOperations(FlowDescription flowDescription, IEnumerable<PerformedBusinessOperation> operations, int packageSize)
        {
            var performedBusinessOperations = operations as PerformedBusinessOperation[] ?? operations.ToArray();
            if (!performedBusinessOperations.Any())
            {
                return;
            }

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                IEnumerable<IExportableEntityDto> failedEntites;
                _operationsExporter.ExportOperations(flowDescription, performedBusinessOperations, packageSize, out failedEntites);

                _processingsStoreService.SaveProcessedOperations(performedBusinessOperations, CreateProcessedOperation, UpdateProcessedOperation);
                _processingsStoreService.InsertToFailureQueue(failedEntites);

                transaction.Complete();
            }
        }

        public void ExportFailedEntities(FlowDescription flowDescription, int packageSize)
        {
            var skipCount = 0;

            while (true)
            {
                var failedEntities = GetFailedEntities(packageSize, skipCount);
                if (!failedEntities.Any())
                {
                    return;
                }

                var exportedCount = ExportFailedEntities(flowDescription, failedEntities, packageSize);
                skipCount += packageSize - exportedCount;
            }
        }

        protected void UpdateProcessedOperation(TProcessedOperationEntity processedOperationEntity)
        {
            processedOperationEntity.Date = DateTime.UtcNow;
        }

        protected TProcessedOperationEntity CreateProcessedOperation(PerformedBusinessOperation operation)
        {
            return new TProcessedOperationEntity { Id = operation.Id, Date = DateTime.UtcNow };
        }

        private IEnumerable<ExportFailedEntity> GetFailedEntities(int maxEntitiesCount, int skipCount)
        {
            return _processingsStoreService.GetFailedEntities(maxEntitiesCount, skipCount);
        }

        private int ExportFailedEntities(FlowDescription flowDescription, IEnumerable<ExportFailedEntity> failedEntities, int packageSize)
        {
            var exportFailedEntities = failedEntities as ExportFailedEntity[] ?? failedEntities.ToArray();
            if (exportFailedEntities.Length == 0)
            {
                return 0;
            }

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                IEnumerable<IExportableEntityDto> exportedEntites;
                _operationsExporter.ExportFailedEntities(flowDescription, exportFailedEntities, packageSize, out exportedEntites);

                _processingsStoreService.RemoveFromFailureQueue(exportedEntites);

                transaction.Complete();
                return exportedEntites.Count();
            }
        }
    }
}
