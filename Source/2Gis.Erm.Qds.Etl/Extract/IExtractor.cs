namespace DoubleGis.Erm.Qds.Etl.Extract
{
    public interface IExtractor
    {
        void Extract(IDataSource dataSource, IDataConsumer consumer);
    }
}