using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Export.Export
{
    public abstract class OperationsExportService<TEntity, TProcessedOperationEntity> : IOperationsExportService
        where TEntity : class, IEntity, IEntityKey
        where TProcessedOperationEntity : class, IEntity, IEntityKey
    {
        private static readonly TimeSpan SafetyTimeFactor = TimeSpan.FromDays(1);

        private readonly IExportableOperationsPersistenceService<TEntity, TProcessedOperationEntity> _persistenceService;
        private readonly IOperationsExporter<TEntity, TProcessedOperationEntity> _operationsExporter;

        protected OperationsExportService(IExportableOperationsPersistenceService<TEntity, TProcessedOperationEntity> persistenceService,
                                          IOperationsExporter<TEntity, TProcessedOperationEntity> operationsExporter)
        {
            _persistenceService = persistenceService;
            _operationsExporter = operationsExporter;
        }

        protected abstract ISelectSpecification<TProcessedOperationEntity, DateTime> ProcessingDateSpecification { get; }

        public IEnumerable<PerformedBusinessOperation> GetPendingOperations(int maxOperationCount)
        {
            var lastProcessedOperationPerformDate = _persistenceService.GetLastProcessedOperationPerformDate(ProcessingDateSpecification);
            if (lastProcessedOperationPerformDate > DateTime.MinValue)
            {
                lastProcessedOperationPerformDate = lastProcessedOperationPerformDate - SafetyTimeFactor;
            }

            return _persistenceService.GetPendingOperations(lastProcessedOperationPerformDate, maxOperationCount);
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

                _persistenceService.SaveProcessedOperations(performedBusinessOperations, CreateProcessedOperation, UpdateProcessedOperation);
                _persistenceService.InsertToFailureQueue(failedEntites);

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

        protected abstract void UpdateProcessedOperation(TProcessedOperationEntity processedOperationEntity);
        protected abstract TProcessedOperationEntity CreateProcessedOperation(PerformedBusinessOperation operation);

        private IEnumerable<ExportFailedEntity> GetFailedEntities(int maxEntitiesCount, int skipCount)
        {
            return _persistenceService.GetFailedEntities(maxEntitiesCount, skipCount);
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

                _persistenceService.RemoveFromFailureQueue(exportedEntites);

                transaction.Complete();
                return exportedEntites.Count();
            }
        }
    }
}
