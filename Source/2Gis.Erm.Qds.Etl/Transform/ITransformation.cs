using DoubleGis.Erm.Qds.Etl.Extract;

namespace DoubleGis.Erm.Qds.Etl.Transform
{
    public interface ITransformation
    {
        void Init();
        void Transform(IData data, ITransformedDataConsumer consumer);
    }
}