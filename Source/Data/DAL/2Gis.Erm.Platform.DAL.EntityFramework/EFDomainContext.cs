using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.API.Core.UseCases.Context;
using DoubleGis.Erm.Platform.API.Core.UseCases.Context.Keys;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces.Integration;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public sealed class EFDomainContext : IModifiableDomainContext, IReadDomainContext
    {
        private const string StoredProcedurePrefix = "Replicate";

        private static readonly List<Type> DeferredReplicationTypes = new List<Type>
            {
                typeof(Firm),
                typeof(Territory),
                typeof(FirmAddress),
            };

        private readonly HashSet<IEntityKey> _replicableHashSet = new HashSet<IEntityKey>();
        private readonly IProcessingContext _processingContext;
        private readonly IDbContext _dbContext;
        private readonly IPendingChangesHandlingStrategy _pendingChangesHandlingStrategy;
        private readonly IMsCrmSettings _msCrmSettings;
        private readonly ICommonLog _logger;

        public EFDomainContext(IProcessingContext processingContext,
                               string defaultContextName,
                               IDbContext dbContext,
                               IPendingChangesHandlingStrategy pendingChangesHandlingStrategy,
                               IMsCrmSettings msCrmSettings,
                               ICommonLog logger)
        {
            DefaultContextName = defaultContextName;

            _processingContext = processingContext;
            _dbContext = dbContext;
            _pendingChangesHandlingStrategy = pendingChangesHandlingStrategy;
            _msCrmSettings = msCrmSettings;
            _logger = logger;

            EnsureUseCaseDuration();
        }

        public string DefaultContextName { get; private set; }

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
        
        public int SaveChanges(SaveOptions options)
        {
            foreach (var entry in _dbContext.Entries())
            {
                var entity = entry.Entity;
                if (entity == null)
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

                if (entry.State != EntityState.Unchanged)
                {
                    MarkEntityAsReplicable(entity);
                }
            }

            EnsureUseCaseDuration();

            var savedCount = _dbContext.SaveChanges(options);
            if (savedCount > 0 && options != SaveOptions.None)
            {   
                // сохранили 
            }

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

        public void AcceptAllChanges()
        {
            _dbContext.AcceptAllChanges();
        }

        public ObjectResult<TElement> ExecuteFunction<TElement>(string functionName, params ObjectParameter[] parameters)
        {
            return _dbContext.ExecuteFunction<TElement>(functionName, parameters);
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
            if (!_msCrmSettings.EnableReplication)
            {
                return;
            }

            if (DeferredReplicationTypes.Contains(entity.GetType()))
            {
                return;
            }
            
            var replicable = entity as IReplicableEntity;
            if (replicable != null)
            {
                _replicableHashSet.Add(replicable);
                return;
            }

            var replicableExplicitly = entity as IReplicableExplicitly;
            if (replicableExplicitly != null)
            {
                _replicableHashSet.Add(replicableExplicitly);
            }
        }

        private void Replicate()
        {
            if (!_replicableHashSet.Any())
            {
                return;
            }

            var replicableHashSetCopy = new IEntityKey[_replicableHashSet.Count];
            _replicableHashSet.CopyTo(replicableHashSetCopy);

            var storedProcedurePrefix = DefaultContextName + "." + StoredProcedurePrefix;

            foreach (var entity in replicableHashSetCopy)
            {
                try
                {
                    _dbContext.ExecuteFunction(storedProcedurePrefix + entity.GetType().Name, new ObjectParameter("Id", entity.Id));
                    _replicableHashSet.Remove(entity);
                }
                catch (Exception ex)
                {
                    _replicableHashSet.Clear();

                    if (_logger != null)
                    {
                        _logger.ErrorEx(ex, string.Format("Произошла ошибка при репликации сущности EntityType=[{0}], Id=[{1}]", entity.GetType().Name, entity.Id));
                    }

                    throw;
                }
            }
        }
    }
}