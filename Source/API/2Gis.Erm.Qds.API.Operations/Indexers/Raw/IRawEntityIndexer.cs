namespace DoubleGis.Erm.Qds.API.Operations.Indexers.Raw
{
    public interface IRawEntityIndexer
    {
        void IndexEntities(string entityType, params long[] ids);
    }
}