using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Futures;
using NuClear.Storage.Specifications;

namespace NuClear.Storage
{
    /// <summary>
    /// Интерфейс для readonly доступа к данным
    /// </summary>
    public interface IFinder 
    {
        /// <summary>
        /// Compose future sequence based on findSpecification.
        /// </summary>
        // FutureSequence<TSource> Find<TSource>(FindSpecification<TSource> findSpecification) where TSource : class, IEntity;
        
        /// <summary>
        /// Find the Entity object(s) based on findSpecification.
        /// </summary>
        IQueryable<TEntity> Find<TEntity>(FindSpecification<TEntity> findSpecification) where TEntity : class, IEntity;

        /// <summary>
        /// Find the Entity object(s) based on findSpecification and returns the projection based on selectSpecification.
        /// </summary>
        IQueryable<TOutput> Find<TEntity, TOutput>(FindSpecification<TEntity> findSpecification, SelectSpecification<TEntity, TOutput> selectSpecification)
            where TEntity : class, IEntity;
        
        /// <summary>
        /// Find the Entity object(s) based on user supplied lambda expression.
        /// </summary>
        IQueryable<TEntity> Find<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class, IEntity;

        /// <summary>
        /// Find one entity object based on findSpecification.
        /// </summary>
        TEntity FindOne<TEntity>(FindSpecification<TEntity> findSpecification) where TEntity : class, IEntity;

        /// <summary>
        /// Find one entity object based on findSpecification and returns the projection based on selectSpecification.
        /// </summary>
        TOutput FindOne<TEntity, TOutput>(FindSpecification<TEntity> findSpecification, SelectSpecification<TEntity, TOutput> selectSpecification)
            where TEntity : class, IEntity;

        /// <summary>
        /// Find and select collection of entity objects based on findSpecification.
        /// </summary>
        IReadOnlyCollection<TEntity> FindMany<TEntity>(FindSpecification<TEntity> findSpecification) where TEntity : class, IEntity;

        /// <summary>
        /// Find and select collection of entity objects based on findSpecification and returns the projection based on selectSpecification.
        /// </summary>
        IReadOnlyCollection<TOutput> FindMany<TEntity, TOutput>(FindSpecification<TEntity> findSpecification, SelectSpecification<TEntity, TOutput> selectSpecification)
            where TEntity : class, IEntity;

        /// <summary>
        /// Find any of entity objects based on findSpecification without materialization.
        /// </summary>
        bool FindAny<TEntity>(FindSpecification<TEntity> findSpecification) where TEntity : class, IEntity;
    }
}
