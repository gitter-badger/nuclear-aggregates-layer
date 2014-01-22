using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Export
{
    public sealed class ExportableOperationsPersistenceService<TEntity, TProcessedOperationEntity> : IExportableOperationsPersistenceService<TEntity, TProcessedOperationEntity>
        where TEntity : class, IEntity, IEntityKey
        where TProcessedOperationEntity : class, IEntity, IEntityKey
    {
        private readonly EntityName _entityName = typeof(TEntity).AsEntityName();

        private readonly IIdentityProvider _identityProvider;
        private readonly IFinder _finder;
        private readonly IRepository<TProcessedOperationEntity> _processedOperationEntity;
        private readonly IRepository<ExportFailedEntity> _exportFailedRepository;

        public ExportableOperationsPersistenceService(IIdentityProvider identityProvider,
                                                      IFinder finder,
                                                      IRepository<TProcessedOperationEntity> processedOperationEntity,
                                                      IRepository<ExportFailedEntity> exportFailedRepository)
        {
            _identityProvider = identityProvider;
            _finder = finder;
            _processedOperationEntity = processedOperationEntity;
            _exportFailedRepository = exportFailedRepository;
        }

        public IEnumerable<PerformedBusinessOperation> GetPendingOperations(DateTime ignoreOperationsPrecedingDate)
        {
            var batchOfOperationsToProcess = GetBatchOfOperationsToProcessQuery(ignoreOperationsPrecedingDate);
            return batchOfOperationsToProcess.ToArray();
        }

        public IEnumerable<PerformedBusinessOperation> GetPendingOperations(DateTime ignoreOperationsPrecedingDate,
                                                                            int maxOperationCount)
        {
            var batchOfOperationsToProcess = GetBatchOfOperationsToProcessQuery(ignoreOperationsPrecedingDate);
            return batchOfOperationsToProcess.OrderBy(operation => operation.Date).Take(maxOperationCount).ToArray();
        }

        public IEnumerable<ExportFailedEntity> GetFailedEntities()
        {
            return GetFailedEntitiesQuery().ToArray();
        }

        public IEnumerable<ExportFailedEntity> GetFailedEntities(int maxEntitiesCount, int skipCount)
        {
            return GetFailedEntitiesQuery()
                .OrderBy(entity => entity.Id)
                .Skip(skipCount)
                .Take(maxEntitiesCount)
                .ToArray();
        }

        /// <summary>
        /// Добавляет идентифкаторы сущностей в очередь на повторную обработку.
        /// </summary>
        public void InsertToFailureQueue(IEnumerable<IExportableEntityDto> failedObjects)
        {
            foreach (var failedObjectId in failedObjects.Select(x => x.Id))
            {
                var entity = new ExportFailedEntity
                    {
                        EntityName = (int)_entityName,
                        EntityId = failedObjectId,
                    };
                _identityProvider.SetFor(entity);
                _exportFailedRepository.Add(entity);
            }

            _exportFailedRepository.Save();
        }

        /// <summary>
        /// Удаляет идентифкаторы сущностей из очереди. Следует вызыфвать после успешной обработки. 
        /// </summary>
        public void RemoveFromFailureQueue(IEnumerable<IExportableEntityDto> exportedObjects)
        {
            var exportedObjectIds = exportedObjects.Select(x => x.Id).ToArray();
            var records = _finder.Find<ExportFailedEntity>(entity => entity.EntityName == (int)_entityName &&
                                                                     exportedObjectIds.Contains(entity.EntityId))
                                 .ToArray();
            foreach (var record in records)
            {
                _exportFailedRepository.Delete(record);
            }

            _exportFailedRepository.Save();
        }

        public int SaveProcessedOperations(IEnumerable<PerformedBusinessOperation> operations,
                                          Func<PerformedBusinessOperation, TProcessedOperationEntity> processedOperationEntityCreator,
                                          Action<TProcessedOperationEntity> processedOperationEntityUpdater)
        {
            var performedBusinessOperations = operations as PerformedBusinessOperation[] ?? operations.ToArray();

            var operationIds = performedBusinessOperations.Select(operation => operation.Id);
            var existing = _finder.Find(Specs.Find.ByIds<TProcessedOperationEntity>(operationIds))
                                  .ToDictionary(result => result.Id, result => result);
            foreach (var operation in performedBusinessOperations)
            {
                TProcessedOperationEntity operationResult;

                if (existing.TryGetValue(operation.Id, out operationResult))
                {
                    processedOperationEntityUpdater(operationResult);
                    _processedOperationEntity.Update(operationResult);
                }
                else
                {
                    operationResult = processedOperationEntityCreator(operation);
                    _processedOperationEntity.Add(operationResult);
                }
            }

            return _processedOperationEntity.Save();
        }

        public DateTime GetLastProcessedOperationPerformDate(ISelectSpecification<TProcessedOperationEntity, DateTime> selectSortFieldSpecification)
        {
            var lastMessageId = _finder.FindAll<TProcessedOperationEntity>()
                                       .OrderByDescending(selectSortFieldSpecification.Selector)
                                       .Select(x => x.Id)
                                       .Take(1);

            var lastDate = _finder.Find<PerformedBusinessOperation>(operation => lastMessageId.Contains(operation.Id))
                                  .Select(operation => operation.Date)
                                  .FirstOrDefault();

            return lastDate == default(DateTime) ? DateTime.MinValue : lastDate;
        }

        private IQueryable<PerformedBusinessOperation> GetBatchOfOperationsToProcessQuery(DateTime ignoreOperationsPrecedingDate)
        {
            var integrationEntityName = typeof(TProcessedOperationEntity).AsEntityName();

            // FIXME {d.ivanov, 23.08.2013}: Полностью перейти от IntegrationService к EntityName
            // comment {a.rechkalov, 2013-11-11}: Нужно в таблице Shared.BusinessOperationServices в колонке Service проставить значения EntityName (например, ExportFlowOrdersOrder вместо "5")
            var integrationService = integrationEntityName.AsIntegrationService();

            var performedBusinessOperations = _finder.FindAll<PerformedBusinessOperation>();
            var processedBusinessOperations = _finder.FindAll<TProcessedOperationEntity>();
            var operationTypesToProcess = _finder.Find<BusinessOperationService>(service => service.Service == (int)integrationService);

            // Сложный запрос, который находит отражение в элементарном LEFT JOIN после трансляции в SQL
            var notExportedOperations = performedBusinessOperations
                .GroupJoin(processedBusinessOperations,
                           performedOperation => performedOperation.Id,
                           processedOperation => processedOperation.Id,
                           (performedOperation, processedOperations) => new { performedOperation, processedOperations })
                .SelectMany(pair => pair.processedOperations.DefaultIfEmpty(),
                            (pair, processedOperation) => new { pair, processedOperation })
                .Where(pair => pair.processedOperation == null)
                .Select(pair => pair.pair.performedOperation);

            var notExportedOperationsThatMustBeProcessed = notExportedOperations
                .Join(operationTypesToProcess,
                      operation => new { operation.Descriptor, operation.Operation },
                      service => new { service.Descriptor, service.Operation },
                      (operation, service) => operation);

            return notExportedOperationsThatMustBeProcessed.Where(operation => operation.Date > ignoreOperationsPrecedingDate);
        }

        private IQueryable<ExportFailedEntity> GetFailedEntitiesQuery()
        {
            return _finder.Find<ExportFailedEntity>(entity => entity.EntityName == (int)_entityName);
        }
    }
}
