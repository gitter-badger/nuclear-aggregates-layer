using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public sealed class EFDbContext : IDbContext
    {
        private readonly DbContext _dbContext;
        private bool _isDisposed;

        public EFDbContext(DbConnection connection, DbCompiledModel model, IProducedQueryLogAccessor producedQueryLogAccessor, bool contextOwnsConnection)
        {
            _dbContext = new DbContext(connection, model, contextOwnsConnection);
            _dbContext.Configuration.ValidateOnSaveEnabled = true;
            _dbContext.Configuration.UseDatabaseNullSemantics = true;
            _dbContext.Configuration.LazyLoadingEnabled = false;
            _dbContext.Configuration.ProxyCreationEnabled = false;
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
            _dbContext.Database.Log = producedQueryLogAccessor.Log;
        }

        public int? CommandTimeout
        {
            get { return _dbContext.Database.CommandTimeout; }
            set { _dbContext.Database.CommandTimeout = value; }
        }

        public IQueryable Set(Type entityType)
        {
            return _dbContext.Set(entityType);
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

        public IEnumerable<IDbEntityEntry> Entries()
        {
            return _dbContext.ChangeTracker.Entries().Select(x => new EFEntityEntry(x));
        }

        public bool HasChanges()
        {
            return !_isDisposed && _dbContext.ChangeTracker.HasChanges();
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _dbContext.Dispose();
            _isDisposed = true;
        }

        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }
    }
}