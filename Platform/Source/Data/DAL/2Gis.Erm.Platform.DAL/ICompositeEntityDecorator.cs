using System;
using System.Linq;
using System.Linq.Expressions;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Platform.DAL
{
    public interface ICompositeEntityDecorator
    {
        IQueryable<TEntity> Find<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class;
        IQueryable<TEntity> Find<TEntity>(FindSpecification<TEntity> findSpecification) where TEntity : class;
    }
}