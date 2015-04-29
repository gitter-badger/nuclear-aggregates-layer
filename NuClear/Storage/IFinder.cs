using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Specifications;

namespace NuClear.Storage
{
    /// <summary>
    /// Интерфейс для readonly доступа к данным
    /// </summary>
    public interface IFinder 
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
        /// Find one entity object based on findSpecification.
        /// </summary>
        TEntity FindOne<TEntity>(IFindSpecification<TEntity> findSpecification) where TEntity : class, IEntity;

        /// <summary>
        /// Find and select collection of entity objects based on findSpecification.
        /// </summary>
        IEnumerable<TEntity> FindMany<TEntity>(IFindSpecification<TEntity> findSpecification) where TEntity : class, IEntity;
    }
}
