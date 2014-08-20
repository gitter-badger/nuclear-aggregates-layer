using System;
using System.Linq;
using System.Linq.Expressions;

namespace DoubleGis.Erm.Platform.DAL
{
	public interface ICompositeEntityDecorator
	{
		IQueryable<TEntity> Find<TEntity>(Expression<Func<TEntity, bool>> predicate);
	}
}