using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.API.Core.UseCases.Context;
using DoubleGis.Erm.Platform.API.Core.UseCases.Context.Keys;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Metadata.Replication.Metadata;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Entities.Aspects.Integration;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public sealed class EFDomainContext : IModifiableDomainContext, IReadDomainContext
    {
        private readonly HashSet<Tuple<IEntityKey, string>> _replicableHashSet = new HashSet<Tuple<IEntityKey, string>>();
        private readonly IProcessingContext _processingContext;
        private readonly string _defaultContextName;
        private readonly IDbContext _dbContext;
        private readonly IPendingChangesHandlingStrategy _pendingChangesHandlingStrategy;
        private readonly IMsCrmReplicationMetadataProvider _msCrmReplicationMetadataProvider;
        private readonly ICommonLog _logger;

        public EFDomainContext(IProcessingContext processingContext,
                               string defaultContextName,
                               IDbContext dbContext,
                               IPendingChangesHandlingStrategy pendingChangesHandlingStrategy,
                               IMsCrmReplicationMetadataProvider msCrmReplicationMetadataProvider,
                               ICommonLog logger)
        {
            _processingContext = processingContext;
            _defaultContextName = defaultContextName;
            _dbContext = dbContext;
            _pendingChangesHandlingStrategy = pendingChangesHandlingStrategy;
            _msCrmReplicationMetadataProvider = msCrmReplicationMetadataProvider;
            _logger = logger;

            EnsureUseCaseDuration();
        }

        public bool AnyPendingChanges
        {
            get
            {
                try
                {
                    return _dbContext.HasChanges();
                }
                catch (InvalidOperationException)
                {
                    // object context is already disposed
                    return false;
                }
            }
        }

        // TODO {all, 07.12.2014}: при поседующей очистке DAL (cинхронная репликация и т.п., можно будет выпилить подерржку saveoptions и AcceptAllChanges у DomainContext)
        int IModifiableDomainContext.SaveChanges(SaveOptions options)
        {
            foreach (var entry in _dbContext.Entries())
            {
                var entity = entry.Entity;
                if (entity == null || entry.State == EntityState.Unchanged)
                {
                    continue;
                }

                if (entry.State == EntityState.Added)
                {
                    var replicateableEntity = entity as IReplicableEntity;
                    if (replicateableEntity != null)
                    {
                        replicateableEntity.ReplicationCode = Guid.NewGuid();
                    }
                }

                MarkEntityAsReplicable(entity);
            }

            EnsureUseCaseDuration();

            var savedCount = _dbContext.SaveChanges(options);

            Replicate();

            return savedCount;
        }

        #region Delegating members for ObjectContext

        public void Dispose()
        {
            if (_dbContext != null)
            {
                _pendingChangesHandlingStrategy.HandlePendingChanges(this);
                _dbContext.Dispose();
            }
        }

        // TODO {all, 07.12.2014}: при поседующей очистке DAL (cинхронная репликация и т.п., можно будет выпилить подерржку saveoptions и AcceptAllChanges у DomainContext)
        void IModifiableDomainContext.AcceptAllChanges()
        {
            _dbContext.AcceptAllChanges();
        }

        public DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return _dbContext.Set<TEntity>();
        }

        public DbEntityEntry Entry(object entity)
        {
            return _dbContext.Entry(entity);
        }

        public DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class
        {
            return _dbContext.Entry(entity);
        }

        #endregion

        IQueryable IReadDomainContext.GetQueryableSource(Type entityType)
        {
            EnsureUseCaseDuration();
            return _dbContext.Set(entityType).AsNoTracking();
        }

        IQueryable<TEntity> IReadDomainContext.GetQueryableSource<TEntity>()
        {
            EnsureUseCaseDuration();
            return _dbContext.Set<TEntity>().AsNoTracking();
        }

        private void EnsureUseCaseDuration()
        {
            // пока конвертация простая - значение enum UseCaseDuration используется как значение command timeout в секундах
            var timeout = (int)(_processingContext.ContainsKey(UseCaseDurationKey.Instance)
                                    ? _processingContext.GetValue(UseCaseDurationKey.Instance)
                                    : UseCaseDuration.Normal);
            var currentValue = _dbContext.CommandTimeout ?? 0;
            _dbContext.CommandTimeout = Math.Max(timeout, currentValue);
        }

        private void MarkEntityAsReplicable(object entity)
        {
            EntityReplicationInfo replicationInfo;
            if (!_msCrmReplicationMetadataProvider.TryGetSyncMetadata(entity.GetType(), out replicationInfo))
            {
                return;
            }

            _replicableHashSet.Add(Tuple.Create((IEntityKey)entity, string.Format("EXEC {0} @Id", replicationInfo.SchemaQualifiedStoredProcedureName)));
        }

        private void Replicate()
        {
            if (!_replicableHashSet.Any())
            {
                return;
            }

            var replicableHashSetCopy = new Tuple<IEntityKey, string>[_replicableHashSet.Count];
            _replicableHashSet.CopyTo(replicableHashSetCopy);

            foreach (var entityReplicationInfo in replicableHashSetCopy)
            {
                var entity = entityReplicationInfo.Item1;
                var replicationProcedureCallSql = entityReplicationInfo.Item2;

                try
                {
                    _dbContext.ExecuteSql(replicationProcedureCallSql, new SqlParameter("Id", entity.Id));
                    _replicableHashSet.Remove(entityReplicationInfo);
                }
                catch (Exception ex)
                {
                    _replicableHashSet.Clear();

                    if (_logger != null)
                    {
                        _logger.Error(ex, string.Format("Произошла ошибка при репликации сущности EntityType=[{0}], Id=[{1}]", entity.GetType().Name, entity.Id));
                    }

                    throw;
                }
            }
        }
    }
}