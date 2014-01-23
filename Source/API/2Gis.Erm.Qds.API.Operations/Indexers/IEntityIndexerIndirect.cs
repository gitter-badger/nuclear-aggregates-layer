using System.Linq;

namespace DoubleGis.Erm.Qds.API.Operations.Indexers
{
    public interface IEntityIndexerIndirect<in TEntity>
    {
        void IndexEntitiesIndirectly(IQueryable<TEntity> query);
    }
}