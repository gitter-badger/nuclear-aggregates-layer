using System.Collections.Generic;

namespace NuClear.Storage.Core
{
    /// <summary>
    /// Абстракция для domaincontext поддерживающего модификацию данных, а следовательно, для него необходимы методы сохранения изменений и т.д.
    /// </summary>
    public interface IModifiableDomainContext : IDomainContext, IPendingChangesMonitorable
    {
        void Add<TEntity>(TEntity entity) where TEntity : class;
        void AddRange<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;
        void Update<TEntity>(TEntity entity) where TEntity : class;
        void Remove<TEntity>(TEntity entity) where TEntity : class;
        void RemoveRange<TEntity>(IEnumerable<TEntity> entitiesToDeletePhysically) where TEntity : class;
        int SaveChanges();
    }
}
