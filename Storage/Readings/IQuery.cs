﻿using System;
using System.Linq;

using NuClear.Storage.Specifications;

namespace NuClear.Storage.Readings
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
        IQueryable<TEntity> For<TEntity>() where TEntity : class;
        
        /// <summary>
        /// Find the all Entity object(s) based on findSpecification
        /// </summary>
        IQueryable<TEntity> For<TEntity>(FindSpecification<TEntity> findSpecification) where TEntity : class;
    }
}