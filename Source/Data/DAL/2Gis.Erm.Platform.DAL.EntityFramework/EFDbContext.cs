﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public class EFDbContext : IDbContext
    {
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Reviewed. Suppression is OK here.")]
        private readonly DbContext _dbContext;
        private bool _isDisposed;

        public EFDbContext(ObjectContext objectContext, IProducedQueryLogAccessor producedQueryLogAccessor)
        {
            _dbContext = new DbContext(objectContext, true);
            _dbContext.Configuration.ValidateOnSaveEnabled = true;
            _dbContext.Configuration.UseDatabaseNullSemantics = true;
            _dbContext.Configuration.LazyLoadingEnabled = false;
            _dbContext.Configuration.ProxyCreationEnabled = false;
            _dbContext.Configuration.AutoDetectChangesEnabled = false;
            _dbContext.Database.Log = producedQueryLogAccessor.Log;
        }

        public int? CommandTimeout
        {
            get { return ((IObjectContextAdapter)_dbContext).ObjectContext.CommandTimeout; }
            set { ((IObjectContextAdapter)_dbContext).ObjectContext.CommandTimeout = value; }
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

        public void AcceptAllChanges()
        {
            ((IObjectContextAdapter)_dbContext).ObjectContext.AcceptAllChanges();
        }

        public int SaveChanges(SaveOptions options)
        {
            return ((IObjectContextAdapter)_dbContext).ObjectContext.SaveChanges(options.ToEFSaveOptions());
        }

        public int ExecuteFunction(string functionName, params ObjectParameter[] parameters)
        {
            return ((IObjectContextAdapter)_dbContext).ObjectContext.ExecuteFunction(functionName, parameters);
        }

        public ObjectResult<TElement> ExecuteFunction<TElement>(string functionName, params ObjectParameter[] parameters)
        {
            return ((IObjectContextAdapter)_dbContext).ObjectContext.ExecuteFunction<TElement>(functionName, parameters);
        }
    }
}
