using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.API.Core.UseCases.Context;
using DoubleGis.Erm.Platform.API.Core.UseCases.Context.Keys;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public sealed class EFDomainContext : IModifiableDomainContext, IReadDomainContext
    {
        private readonly IProcessingContext _processingContext;
        private readonly DbContext _dbContext;
        private readonly IPendingChangesHandlingStrategy _pendingChangesHandlingStrategy;

        private readonly ISet<object> _attachedEntitiesRegistrar = new HashSet<object>();

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
            Attach(entity, EntityState.Added);
        }

        public void AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            foreach (var entity in entities)
            {
                Attach(entity, EntityState.Added);
            }
        }

        public void Update<TEntity>(TEntity entity) where TEntity : class
        {
            Attach(entity, EntityState.Modified);
        }

        public void Remove<TEntity>(TEntity entity) where TEntity : class
        {
            Attach(entity, EntityState.Deleted);
        }

        public void RemoveRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            foreach (var entity in entities)
            {
                Attach(entity, EntityState.Deleted);
            }
        }

        public int SaveChanges()
        {
            EnsureUseCaseDuration();

            var rez = _dbContext.SaveChanges();

            foreach (var entry in _dbContext.ChangeTracker.Entries())
            {
                entry.State = EntityState.Detached;
            }

            _attachedEntitiesRegistrar.Clear();

            return rez;
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

        private void Attach<TEntity>(TEntity entity, EntityState entityState)
            where TEntity : class
        {
            if (_attachedEntitiesRegistrar.Contains(entity))
            {
                var entityKey = entity as IEntityKey;

                // т.е. для экземпяра выполнили CUD, не сохранили и опять пытаемся менять экземпляр с тем же identity
                throw new InvalidOperationException(string.Format("Instance of type {0} with id={1} already in domain context cache " +
                                                                    "with unsaved changes => trying to update not saved entity. " +
                                                                    "Possible entity repository save method not called before next update. " +
                                                                    "Save mode is immediately, not deferred",
                                                                    typeof(TEntity).Name,
                                                                    entityKey != null ? entityKey.Id.ToString() : "NOTDETECTED"));
            }

            _attachedEntitiesRegistrar.Add(entity);

            var entry = _dbContext.Entry(entity);
            _dbContext.Set<TEntity>().Attach(entity);
            entry.State = entityState;
        }
    }
}