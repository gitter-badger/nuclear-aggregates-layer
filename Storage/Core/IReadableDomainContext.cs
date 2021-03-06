﻿using System;
using System.Linq;

namespace NuClear.Storage.Core
{
    /// <summary>
    /// Абстракция domain контекста поддерживающего только чтение
    /// </summary>
    public interface IReadableDomainContext : IDomainContext
    {
        IQueryable GetQueryableSource(Type entityType);
        IQueryable<TEntity> GetQueryableSource<TEntity>() where TEntity : class;
    }
}
