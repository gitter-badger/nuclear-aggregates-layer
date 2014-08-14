using System.Collections.Generic;

namespace DoubleGis.Erm.Platform.DAL
{
	public interface ICompositeEntityDecorator
	{
		IEnumerable<TEntity> Find<TEntity>(params long[] ids);
	}
}