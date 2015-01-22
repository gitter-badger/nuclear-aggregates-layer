﻿using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.DAL
{
    /// <summary>
    /// Абстракция для domaincontext поддерживающего модификацию данных, а следовательно, для него необходимы методы сохранения изменений и т.д.
    /// </summary>
    public interface IModifiableDomainContext : IDomainContext, IPendingChangesMonitorable
    {

        void Add<TEntity>(TEntity entity) where TEntity : class;
        void AddRange<TEntity>(IEnumerable<TEntity> castedEntities) where TEntity : class;
        void Update<TEntity>(TEntity entity) where TEntity : class;
        void Remove<TEntity>(TEntity entity) where TEntity : class;
        void RemoveRange<TEntity>(IEnumerable<TEntity> entitiesToDeletePhysically) where TEntity : class;
        int SaveChanges();
    }
}
