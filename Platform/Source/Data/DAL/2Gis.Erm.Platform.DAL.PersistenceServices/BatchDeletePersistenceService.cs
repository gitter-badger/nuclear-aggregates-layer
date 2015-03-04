using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

using DoubleGis.Erm.Platform.DAL.AdoNet;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.Platform.DAL.PersistenceServices
{
    public sealed class BatchDeletePersistenceService : IBatchDeletePersistenceService
    {
        private readonly IDatabaseCaller _databaseCaller;
        private readonly ITracer _tracer;

        private readonly IReadOnlyDictionary<Type, string> _entitiesTableMap =
            new Dictionary<Type, string>
                {
                    { typeof(PerformedOperationFinalProcessing), "Shared.PerformedOperationFinalProcessings" },
                    { typeof(PerformedOperationPrimaryProcessing), "Shared.PerformedOperationPrimaryProcessings" },
                    { typeof(OrderValidationCacheEntry), "Shared.OrderValidationCacheEntries" },
                    { typeof(SalesModelCategoryRestriction), "BusinessDirectory.SalesModelCategoryRestrictions" },
                };

        public BatchDeletePersistenceService(IDatabaseCaller databaseCaller, ITracer tracer)
        {
            _databaseCaller = databaseCaller;
            _tracer = tracer;
        }

        public void Delete<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntity, IEntityKey
        {
            var entityType = typeof(TEntity);
            string targetEntityTable;
            EvaluateTargetEntityTable(entityType, out targetEntityTable);

            int entitiesCount = 0;
            var sb = new StringBuilder("DELETE FROM ")
                .Append(targetEntityTable)
                .Append(" WHERE Id in (");

            foreach (var entity in entities)
            {
                var entityId = entity.Id.ToString();
                sb.Append(entitiesCount == 0 ? entityId : "," + entityId);
                ++entitiesCount;
            }

            sb.Append(")");

            ExecuteDelete(entityType, entitiesCount, sb.ToString());
        }

        public void Delete<TEntity>(
            IEnumerable<TEntity> entities,
            IReadOnlyList<EntityKeyExtractor<TEntity>> keyExtractors) where TEntity : class, IEntity
        {
            var entityType = typeof(TEntity);
            string targetEntityTable;
            EvaluateTargetEntityTable(entityType, out targetEntityTable);

            int entitiesCount = 0;
            var sb = new StringBuilder("DELETE FROM ")
                .Append(targetEntityTable)
                .Append(" WHERE ");

            foreach (var entity in entities)
            {
                var entityKeysCondition = ExtractKeysFilterExpression(entity, keyExtractors);
                sb.Append(entitiesCount == 0 ? entityKeysCondition : " OR " + entityKeysCondition);
                ++entitiesCount;
            }

            ExecuteDelete(entityType, entitiesCount, sb.ToString());
        }

        private static string ExtractKeysFilterExpression<TEntity>(TEntity entity, IReadOnlyList<EntityKeyExtractor<TEntity>> keyExtractors) 
            where TEntity : class, IEntity
        {
            const string SingleKeyConditionTemplate = "{0} = {1}";
            var sb = new StringBuilder();

            sb.Append("(");

            var firstExtractor = keyExtractors[0];
            sb.AppendFormat(SingleKeyConditionTemplate, firstExtractor.KeyName, firstExtractor.KeyValueExtractor(entity));

            for (int i = 1; i < keyExtractors.Count; i++)
            {
                sb.Append(" AND ").AppendFormat(SingleKeyConditionTemplate, keyExtractors[i].KeyName, keyExtractors[i].KeyValueExtractor(entity));
            }

            return sb.Append(")").ToString();
        }

        private void EvaluateTargetEntityTable(Type entityType, out string targetEntityTable)
        {
            if (_entitiesTableMap.TryGetValue(entityType, out targetEntityTable))
            {
                return;
            }

            var msg = string.Format("Specified entity type {0} doesn't supported batch delete behavior", entityType);
            _tracer.Error(msg);
            throw new InvalidOperationException(msg);
        }

        private void ExecuteDelete(Type entityType, int entitiesCount, string deleteCommandBatchText)
        {
            if (entitiesCount == 0)
            {
                _tracer.WarnFormat("Batch delete of entity {0} was skipped, because target entities list was empty", entityType);
                return;
            }

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                try
                {
                    _databaseCaller.ExecuteRawSql(deleteCommandBatchText);
                }
                catch (Exception ex)
                {
                    _tracer.ErrorFormat(ex, "Can't execute batch delete of entities with type {0}, specified entities count {1}", entityType, entitiesCount);
                    throw;
                }

                transaction.Complete();
            }
        }
    }
}