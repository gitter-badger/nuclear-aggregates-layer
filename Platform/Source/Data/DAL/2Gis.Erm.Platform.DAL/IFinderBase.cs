using System;
using System.Linq;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.DAL
{
    public interface IFinderBase
    {
        /// <summary>
        /// Find the all generic object(s)
        /// </summary>
        IQueryable FindAll(Type entityType);

        /// <summary>
        /// Find the all Entity object(s)
        /// </summary>
        IQueryable<TEntity> FindAll<TEntity>() where TEntity : class, IEntity;
    }
}