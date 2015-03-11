using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.API.Core.UseCases.Context;
using DoubleGis.Erm.Platform.API.Core.UseCases.Context.Keys;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public sealed class EFDomainContext : IModifiableDomainContext, IReadDomainContext
    {
        private readonly IProcessingContext _processingContext;
        private readonly DbContext _dbContext;
        private readonly IPendingChangesHandlingStrategy _pendingChangesHandlingStrategy;

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
                }

        public void AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
                {
            _dbContext.Set<TEntity>().AddRange(entities);
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
            // physically delete from database
            _dbContext.Set<TEntity>().Remove(GetAttachedEntity(entity));
        }

        public void RemoveRange<TEntity>(IEnumerable<TEntity> entitiesToDeletePhysically) where TEntity : class
        {
            _dbContext.Set<TEntity>().RemoveRange(entitiesToDeletePhysically.Select(GetAttachedEntity).ToArray());
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
            var existingEntry = _dbContext.ChangeTracker.Entries<TEntity>().SingleOrDefault(x => x.Entity.Equals(entity));
            if (existingEntry != null)
        {
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

            var entry = _dbContext.Entry(entity);
            if (entry.State == EntityState.Detached)
                    {
                _dbContext.Set<TEntity>().Attach(entity);
                    }

            dbEntityEntry = entry;
            return true;
        }
    }
}