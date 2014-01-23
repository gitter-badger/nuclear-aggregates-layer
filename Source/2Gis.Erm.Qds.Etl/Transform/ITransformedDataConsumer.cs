namespace DoubleGis.Erm.Qds.Etl.Transform
{
    public interface ITransformedDataConsumer
    {
        void DataTransformed(ITransformedData transformedData);
    }
}