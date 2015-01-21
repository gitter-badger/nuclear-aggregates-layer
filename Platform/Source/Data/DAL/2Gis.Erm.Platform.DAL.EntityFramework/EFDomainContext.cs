using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.API.Core.UseCases.Context;
using DoubleGis.Erm.Platform.API.Core.UseCases.Context.Keys;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces.Integration;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public sealed class EFDomainContext : IModifiableDomainContext, IReadDomainContext
    {
        private readonly IProcessingContext _processingContext;
        private readonly IDbContext _dbContext;
        private readonly IPendingChangesHandlingStrategy _pendingChangesHandlingStrategy;

        public EFDomainContext(IProcessingContext processingContext,
                               IDbContext dbContext,
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
                    return _dbContext.HasChanges();
                }
                catch (InvalidOperationException)
                {
                    // object context is already disposed
                    return false;
                }
            }
        }

        int IModifiableDomainContext.SaveChanges()
        {
            foreach (var entry in _dbContext.Entries())
            {
                var replicateableEntity = entry.Entity as IReplicableEntity;
                if (replicateableEntity != null && entry.State == EntityState.Added)
                {
                    replicateableEntity.ReplicationCode = Guid.NewGuid();
                }
            }

            EnsureUseCaseDuration();

            return _dbContext.SaveChanges();
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

        public DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return _dbContext.Set<TEntity>();
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
    }
}