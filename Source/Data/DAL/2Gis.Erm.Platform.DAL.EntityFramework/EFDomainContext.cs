using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
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

        private readonly HashSet<IEntityKey> _replicableHashSet = new HashSet<IEntityKey>();

        private readonly IProcessingContext _processingContext;
        private readonly IObjectContext _objectContext;
        private readonly IPendingChangesHandlingStrategy _pendingChangesHandlingStrategy;
        private readonly IMsCrmSettings _msCrmSettings;
        private readonly ICommonLog _logger;

        private static readonly List<Type> DeferredReplicationTypes = new List<Type>
        {
            typeof(Firm),
            typeof(Territory),
            typeof(FirmAddress),
        };

        public EFDomainContext(IProcessingContext processingContext,
                               string defaultContextName,
                               IObjectContext objectContext,
                               IPendingChangesHandlingStrategy pendingChangesHandlingStrategy,
                               IMsCrmSettings msCrmSettings,
                               ICommonLog logger)
        {
            DefaultContextName = defaultContextName;

            _processingContext = processingContext;
            _objectContext = objectContext;
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
                _objectContext.DetectChanges();
                return _objectContext.GetObjectStateEntries(EntityState.Added | EntityState.Modified | EntityState.Deleted).Any();
            }
        }
        
        public int SaveChanges(SaveOptions options)
        {
            _objectContext.DetectChanges();

            var entitiesQuery = _objectContext.GetObjectStateEntries(EntityState.Added | EntityState.Modified | EntityState.Deleted)
                .Where(entry => !entry.IsRelationship)
                .OrderBy(entry => entry.State);

            foreach (var entry in entitiesQuery)
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

            var savedCount = _objectContext.SaveChanges(options.ToEFSaveOptions());
            if (savedCount > 0 && options != SaveOptions.None)
            {   
                // сохранили 
            }

            Replicate();

            return savedCount;
        }

        public void ChangeObjectState(object entity, EntityState state)
        {
            _objectContext.ChangeObjectState(entity, state);
        }

        #region Delegating members for ObjectContext

        public void Dispose()
        {
            if (_objectContext != null)
            {
                _pendingChangesHandlingStrategy.HandlePendingChanges(this);
                _objectContext.Dispose();
            }
        }

        public void AcceptAllChanges()
        {
            _objectContext.AcceptAllChanges();
        }

        public ObjectResult<TElement> ExecuteFunction<TElement>(string functionName, params ObjectParameter[] parameters)
        {
            return _objectContext.ExecuteFunction<TElement>(functionName, parameters);
        }

        public IObjectSet<TEntity> CreateObjectSet<TEntity>() where TEntity : class
        {
            return _objectContext.CreateObjectSet<TEntity>();
        }

        public object GetObjectByKey(EntityKey key)
        {
            return _objectContext.GetObjectByKey(key);
        }

        #endregion

        public void TryGetObjectStateEntry(EntityKey getEntityKey, out EFEntityStateEntry stateEntry)
        {
            _objectContext.TryGetObjectStateEntry(getEntityKey, out stateEntry);
        }

        IQueryable IReadDomainContext.GetQueryableSource(Type entityType)
        {
            EnsureUseCaseDuration();
            return _objectContext.CreateQuery(entityType);
        }

        IQueryable<TEntity> IReadDomainContext.GetQueryableSource<TEntity>()
        {
            EnsureUseCaseDuration();
            var objectSet = _objectContext.CreateObjectSet<TEntity>();
            objectSet.MergeOption = MergeOption.NoTracking;
            return objectSet.AsQueryable();
        }

        private void EnsureUseCaseDuration()
        {
            // пока конвертация простая - значение enum UseCaseDuration используется как значение command timeout в секундах
            var timeout = (int)(_processingContext.ContainsKey(UseCaseDurationKey.Instance)
                                                ? _processingContext.GetValue(UseCaseDurationKey.Instance)
                                                : UseCaseDuration.Normal);
            var currentValue = _objectContext.CommandTimeout ?? 0;
            _objectContext.CommandTimeout = Math.Max(timeout, currentValue);
        }

        private void MarkEntityAsReplicable(object entity)
        {
            if (!_msCrmSettings.EnableReplication)
            {
                return;
            }

            // TODO {y.baranihin, 06.0.2013}: Contains всё же понятнее чем Any(x => x == y)
            // Done
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
                    _objectContext.ExecuteFunction(storedProcedurePrefix + entity.GetType().Name, new ObjectParameter("Id", entity.Id));
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