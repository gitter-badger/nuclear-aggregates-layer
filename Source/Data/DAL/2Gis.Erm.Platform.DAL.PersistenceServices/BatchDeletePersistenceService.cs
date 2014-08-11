using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL.AdoNet;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.DAL.PersistenceServices
{
    public sealed class BatchDeletePersistenceService : IBatchDeletePersistenceService
    {
        private readonly IDatabaseCaller _databaseCaller;
        private readonly ICommonLog _logger;

        private readonly IReadOnlyDictionary<Type, string> _entitiesTableMap =
            new Dictionary<Type, string>
                {
                    { typeof(PerformedOperationFinalProcessing), "Shared.PerformedOperationFinalProcessings" },
                };

        public BatchDeletePersistenceService(IDatabaseCaller databaseCaller, ICommonLog logger)
        {
            _databaseCaller = databaseCaller;
            _logger = logger;
        }

        public void Delete<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntity, IEntityKey
        {
            var entityType = typeof(TEntity);
            string targetEntityTable;
            if (!_entitiesTableMap.TryGetValue(entityType, out targetEntityTable))
            {
                var msg = string.Format("Specified entity type {0} doesn't supported batch delete behavior", entityType);
                _logger.ErrorEx(msg);
                throw new InvalidOperationException(msg);
            }

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

            if (entitiesCount == 0)
            {
                _logger.WarnFormatEx("Batch delete of entity {0} was skipped, because target entities list was empty", entityType);
                return;
            }

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                try
                {
                    var deleteStatement = sb.ToString();
                    _databaseCaller.ExecuteRawSql(deleteStatement);
                }
                catch (Exception ex)
                {
                    _logger.ErrorFormatEx(ex, "Can't execute batch delete of entities with type {0}, specified entities count {1}", entityType, entitiesCount);
                    throw;
                }

                transaction.Complete();
            }
        }
    }
}