using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Core;
using NuClear.Storage.UseCases;

namespace NuClear.Storage.EntityFramework
{
    public sealed class EFDomainContext : IModifiableDomainContext, IReadDomainContext
    {
        private readonly IProcessingContext _processingContext;
        private readonly DbContext _dbContext;
        private readonly IPendingChangesHandlingStrategy _pendingChangesHandlingStrategy;

        private readonly IDictionary<object, object> _dbEntityEntriesCache = new Dictionary<object, object>();

        public EFDomainContext(IProcessingContext processingContext,
                               DbContext dbContext,
                               IPendingChangesHandlingStrategy pendingChangesHandlingStrategy)
        {
            _processingContext = processingContext;
            _dbContext = dbContext;
            _pendingChangesHandlingStrategy = pendingChangesHandlingStrategy;

            EnsureUseCaseDuration();
        }

        public bool AnyPendingChanges
        {
            get
            {
                try
                {
                    return _dbContext.ChangeTracker.HasChanges();
                }
                catch (InvalidOperationException)
                {
                    // object context is already disposed
                    return false;
                }
            }
        }

        public void Add<TEntity>(TEntity entity) where TEntity : class
        {
            _dbContext.Set<TEntity>().Add(entity);

            // TODO {all, 19.03.2015}: Могут возникнуть проблемы для сущностей с автогенеренными id - возможно для них стоит по-другому реализовать Equals/GetHashCode
            _dbEntityEntriesCache.Add(entity, _dbContext.Entry(entity));
        }

        public void AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            _dbContext.Set<TEntity>().AddRange(entities);
            foreach (var entity in entities)
            {
                // TODO {all, 19.03.2015}: Могут возникнуть проблемы для сущностей с автогенеренными id - возможно для них стоит по-другому реализовать Equals/GetHashCode
                _dbEntityEntriesCache.Add(entity, _dbContext.Entry(entity));
            }
        }

        public void Update<TEntity>(TEntity entity) where TEntity : class
        {
            DbEntityEntry<TEntity> entry;
            if (!AttachEntity(entity, out entry))
            {
                entry.CurrentValues.SetValues(entity);
            }

            entry.State = EntityState.Modified;
        }

        public void Remove<TEntity>(TEntity entity) where TEntity : class
        {
            _dbContext.Set<TEntity>().Remove(GetAttachedEntity(entity));
            _dbEntityEntriesCache.Remove(entity);
        }

        public void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            _dbContext.Set<TEntity>().RemoveRange(entities.Select(GetAttachedEntity).ToArray());
            foreach (var entity in entities)
            {
                _dbEntityEntriesCache.Remove(entity);
            }
        }

        public int SaveChanges()
        {
            EnsureUseCaseDuration();

            return _dbContext.SaveChanges();
            }

        public IQueryable GetQueryableSource(Type entityType)
        {
            EnsureUseCaseDuration();
            return _dbContext.Set(entityType).AsNoTracking();
        }

        public IQueryable<TEntity> GetQueryableSource<TEntity>() where TEntity : class
        {
            EnsureUseCaseDuration();
            return _dbContext.Set<TEntity>().AsNoTracking();
        }

        public void Dispose()
        {
            if (_dbContext != null)
            {
                _pendingChangesHandlingStrategy.HandlePendingChanges(this);
                _dbContext.Dispose();
            }
        }

        private void EnsureUseCaseDuration()
        {
            // пока конвертация простая - значение enum UseCaseDuration используется как значение command timeout в секундах
            var timeout = (int)(_processingContext.ContainsKey(UseCaseDurationKey.Instance)
                                    ? _processingContext.GetValue(UseCaseDurationKey.Instance)
                                    : UseCaseDuration.Normal);
            var currentValue = _dbContext.Database.CommandTimeout ?? 0;
            _dbContext.Database.CommandTimeout = Math.Max(timeout, currentValue);
        }

        private TEntity GetAttachedEntity<TEntity>(TEntity entity) where TEntity : class
        {
            DbEntityEntry<TEntity> entry;
            AttachEntity(entity, out entry);
            return entry.Entity;
        }

        private bool AttachEntity<TEntity>(TEntity entity, out DbEntityEntry<TEntity> dbEntityEntry) where TEntity : class
        {
            object entry;
            if (_dbEntityEntriesCache.TryGetValue(entity, out entry))
            {
                var existingEntry = (DbEntityEntry<TEntity>)entry;
                if (existingEntry.State != EntityState.Unchanged)
            {
                    var entityKey = entity as IEntityKey;

                    // используется НЕотложенное сохранение - т.е. объект изменили, не сохранили изменения и опять пытаемся менять экземпляр сущности с тем же identity
                    throw new InvalidOperationException(string.Format("Instance of type {0} with id={1} already in domain context cache " +
                                                                      "with unsaved changes => trying to update not saved entity. " +
                                                                      "Possible entity repository save method not called before next update. " +
                                                                      "Save mode is immediately, not deferred",
                                                                      typeof(TEntity).Name,
                                                                      entityKey != null ? entityKey.Id.ToString() : "NOTDETECTED"));
                }

                dbEntityEntry = existingEntry;
                return false;
            }

            var newEntry = _dbContext.Entry(entity);
            if (newEntry.State == EntityState.Detached)
            {
                _dbContext.Set<TEntity>().Attach(entity);
            }

            _dbEntityEntriesCache.Add(entity, newEntry);
            dbEntityEntry = newEntry;
            return true;
        }
    }
}