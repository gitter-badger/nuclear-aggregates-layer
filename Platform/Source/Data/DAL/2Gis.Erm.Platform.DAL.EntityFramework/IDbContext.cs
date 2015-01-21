using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace DoubleGis.Erm.Platform.DAL.EntityFramework
{
    public interface IDbContext : IDisposable
    {
        int? CommandTimeout { get; set; }

        IQueryable Set(Type entityType);
        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        DbEntityEntry Entry(object entity);
        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        IEnumerable<IDbEntityEntry> Entries();

        bool HasChanges();
        int SaveChanges();
    }
}
