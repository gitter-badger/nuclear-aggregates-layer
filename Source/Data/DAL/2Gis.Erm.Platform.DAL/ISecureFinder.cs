﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.DAL
{
    public interface ISecureFinder : IFinderBase
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
        TEntity FindOne<TEntity>(IFindSpecification<TEntity> findSpecification) where TEntity : class, IEntity, IEntityKey;

        /// <summary>
        /// Find and select collection of entity objects based on findSpecification.
        /// </summary>
        IReadOnlyCollection<TEntity> FindMany<TEntity>(IFindSpecification<TEntity> findSpecification) where TEntity : class, IEntity, IEntityKey;
    }
}
