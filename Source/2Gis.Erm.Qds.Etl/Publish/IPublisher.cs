using DoubleGis.Erm.Qds.Etl.Transform;

namespace DoubleGis.Erm.Qds.Etl.Publish
{
    public interface IPublisher
    {
        void Publish(ITransformedData transformedData);
    }
}