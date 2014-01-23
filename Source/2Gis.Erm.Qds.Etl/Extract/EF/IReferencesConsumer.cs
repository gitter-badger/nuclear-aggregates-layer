namespace DoubleGis.Erm.Qds.Etl.Extract.EF
{
    public interface IReferencesConsumer
    {
        void ReferencesBuilt(IDataSource dataSource);
    }
}