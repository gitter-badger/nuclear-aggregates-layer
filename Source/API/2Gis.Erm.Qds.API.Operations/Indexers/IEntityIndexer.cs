namespace DoubleGis.Erm.Qds.API.Operations.Indexers
{
    public interface IEntityIndexer<in TEntity>
    {
        void IndexEntities(params long[] ids);
    }
}