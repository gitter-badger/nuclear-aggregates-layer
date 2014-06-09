using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.DAL
{
    /// <summary>
    /// Интерфейс для readonly доступа к данным
    /// </summary>
    public interface IFinder : IFinderBase
    {
        /// <summary>
        /// Find the Entity object(s) based on findSpecification.
        /// </summary>
        IQueryable<TEntity> Find<TEntity>(IFindSpecification<TEntity> findSpecification) where TEntity : class, IEntity;

        /// <summary>
        /// Find the Entity object(s) based on findSpecification and returns the projection based on selectSpecification.
        /// </summary>
        IQueryable<TOutput> Find<TEntity, TOutput>(ISelectSpecification<TEntity, TOutput> selectSpecification, IFindSpecification<TEntity> findSpecification)
            where TEntity : class, IEntity;
        
        /// <summary>
        /// Find the Entity object(s) based on user supplied lambda expression.
        /// </summary>
        IQueryable<TEntity> Find<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class, IEntity;

        /// <summary>
        /// Find and select IPartable object with all its parts.
        /// </summary>
        TPartableEntity FindOne<TPartableEntity>(IFindSpecification<TPartableEntity> findSpecification)
            where TPartableEntity : class, IEntity, IEntityKey;

        /// <summary>
        /// Find and select collection of IPartable object with all its parts.
        /// </summary>
        IReadOnlyCollection<TEntity> FindMany<TEntity>(IFindSpecification<TEntity> findSpecification)
            where TEntity : class, IEntity, IEntityKey;
    }
}
