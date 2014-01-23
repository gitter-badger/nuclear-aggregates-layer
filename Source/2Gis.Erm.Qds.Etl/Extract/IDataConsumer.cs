namespace DoubleGis.Erm.Qds.Etl.Extract
{
    public interface IDataConsumer
    {
        void DataExtracted(IData data);
    }
}