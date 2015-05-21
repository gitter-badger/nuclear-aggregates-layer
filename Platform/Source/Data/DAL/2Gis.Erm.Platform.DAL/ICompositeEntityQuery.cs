using System.Linq;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.Platform.DAL
{
    public interface ICompositeEntityQuery
    {
        IQueryable<TEntity> For<TEntity>(FindSpecification<TEntity> findSpecification) where TEntity : class;
    }
}