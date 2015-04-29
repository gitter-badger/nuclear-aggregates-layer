using System;
using System.Linq;

using NuClear.Model.Common.Entities.Aspects;

namespace NuClear.Storage
{
    public interface IQuery
    {
        /// <summary>
        /// Find the all generic object(s)
        /// </summary>
        IQueryable For(Type entityType);

        /// <summary>
        /// Find the all Entity object(s)
        /// </summary>
        IQueryable<TEntity> For<TEntity>() where TEntity : class, IEntity;
    }
}