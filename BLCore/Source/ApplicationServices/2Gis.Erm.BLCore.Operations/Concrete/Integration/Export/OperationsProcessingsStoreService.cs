using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Entities.Aspects.Integration;
using NuClear.Storage;
using NuClear.Storage.Specifications;
using NuClear.Storage.UseCases;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Export
{
    // FIXME {all, 26.03.2014}: данный тип нужно распилить на ReadModel + несколько AggregateService
    // Однако, возможно не стоит с этим торопится, т.к. при впиливании внутреннего транспорта для perfomedbusinessoperations (например, servicebus) - функциональность данного типа становиться очень ограниченной - фактически, 
    // вместо него должен появиться некий receiver сообщений из шины ERM
    // COMMENT {i.maslennikov, 09.04.2014}: Думаю, стоит рассматривать экспорт объектов как отдельный bounded context и выделить в нем агрегаты
    //                                      Важно, что эти выденные агрегаты точно никак не связаны с агрегатами других bounded context-ов
    // TODO {d.ivanov,i.maslennikov, 09.04.2014}: Реализовать в ERM возможность существования нескольких bounded context-ов и некольких непересекающихся наборов агрегатов в них
    [UseCase(Duration = UseCaseDuration.Long)]
    public sealed class OperationsProcessingsStoreService<TEntity, TProcessedOperationEntity> : IOperationsProcessingsStoreService<TEntity, TProcessedOperationEntity>
        where TEntity : class, IEntity, IEntityKey
        where TProcessedOperationEntity : class, IIntegrationProcessorState
    {
        private readonly IEntityType _entityName = typeof(TEntity).AsEntityName();
        private readonly IEntityType _integrationProcessorStateEntityName = typeof(TProcessedOperationEntity).AsEntityName();

        private readonly IQuery _query;
        private readonly IIdentityProvider _identityProvider;
        private readonly IUseCaseTuner _useCaseTuner;
        private readonly IFinder _finder;
        private readonly IRepository<TProcessedOperationEntity> _processedOperationEntity;
        private readonly IRepository<ExportFailedEntity> _exportFailedRepository;

        public OperationsProcessingsStoreService(
            IQuery query,
            IFinder finder,
            IRepository<TProcessedOperationEntity> processedOperationEntity,
            IRepository<ExportFailedEntity> exportFailedRepository,
            IIdentityProvider identityProvider,
            IUseCaseTuner useCaseTuner)
        {
            _query = query;
            _identityProvider = identityProvider;
            _useCaseTuner = useCaseTuner;
            _finder = finder;
            _processedOperationEntity = processedOperationEntity;
            _exportFailedRepository = exportFailedRepository;
        }

        public IEnumerable<PerformedBusinessOperation> GetPendingOperations(DateTime ignoreOperationsPrecedingDate)
        {
            _useCaseTuner.AlterDuration<OperationsProcessingsStoreService<TEntity, TProcessedOperationEntity>>();

            var batchOfOperationsToProcess = GetBatchOfOperationsToProcessQuery(ignoreOperationsPrecedingDate);
            return batchOfOperationsToProcess.ToArray();
        }

        public IEnumerable<PerformedBusinessOperation> GetPendingOperations(DateTime ignoreOperationsPrecedingDate,
                                                                            int maxOperationCount)
        {
            _useCaseTuner.AlterDuration<OperationsProcessingsStoreService<TEntity, TProcessedOperationEntity>>();

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
                        EntityName = _entityName.Id,
                        EntityId = failedObjectId,
                        ProcessorId = _integrationProcessorStateEntityName.Id
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
            var entityTypeId = _entityName.Id;
            var integrationProcessorStateEntityTypeId = _integrationProcessorStateEntityName.Id;
            var exportedObjectIds = exportedObjects.Select(x => x.Id).ToArray();
            var records = _finder.Find<ExportFailedEntity>(entity => entity.EntityName == entityTypeId
                                                                        && entity.ProcessorId == integrationProcessorStateEntityTypeId
                                                                        && exportedObjectIds.Contains(entity.EntityId))
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
            var lastMessageId = _query.For<TProcessedOperationEntity>()
                                      .OrderByDescending(selectSortFieldSpecification.Selector)
                                      .Select(x => x.Id)
                                      .Take(1);

            var lastDate = _finder.Find<PerformedBusinessOperation>(operation => lastMessageId.Contains(operation.Id))
                                  .Select(operation => operation.Date)
                                  .FirstOrDefault();

            return lastDate == default(DateTime) ? DateTime.UtcNow : lastDate;
        }

        private IQueryable<PerformedBusinessOperation> GetBatchOfOperationsToProcessQuery(DateTime ignoreOperationsPrecedingDate)
        {
            var integrationEntityName = typeof(TProcessedOperationEntity).AsEntityName();

            // FIXME {d.ivanov, 23.08.2013}: Полностью перейти от IntegrationService к EntityName
            // comment {a.rechkalov, 2013-11-11}: Нужно в таблице Shared.BusinessOperationServices в колонке Service проставить значения EntityName (например, ExportFlowOrdersOrder вместо "5")
            var integrationService = integrationEntityName.AsIntegrationService();

            var performedBusinessOperations = _query.For<PerformedBusinessOperation>();
            var processedBusinessOperations = _query.For<TProcessedOperationEntity>();
            var operationTypesToProcess = _finder.Find<BusinessOperationService>(service => service.Service == (int)integrationService);

            var notExportedOperations = from performedOperation in performedBusinessOperations
                                        join processedOperation in processedBusinessOperations on performedOperation.Id equals processedOperation.Id
                                            into joinedOperations
                                        from joinedOperation in joinedOperations.DefaultIfEmpty()
                                        where joinedOperation == null
                                        select performedOperation;

            var notExportedOperationsThatMustBeProcessed = from operation in notExportedOperations
                                                           join operationType in operationTypesToProcess on
                                                               new { operation.Descriptor, operation.Operation } equals
                                                               new { operationType.Descriptor, operationType.Operation }
                                                           select operation;
            
            return notExportedOperationsThatMustBeProcessed.Where(operation => operation.Date > ignoreOperationsPrecedingDate);
        }

        private IQueryable<ExportFailedEntity> GetFailedEntitiesQuery()
        {
            var entityTypeId = _entityName.Id;
            var integrationProcessorStateEntityTypeId = _integrationProcessorStateEntityName.Id;
            return _finder.Find<ExportFailedEntity>(entity => entity.EntityName == entityTypeId
                                                                && entity.ProcessorId == integrationProcessorStateEntityTypeId);
        }
    }
}
