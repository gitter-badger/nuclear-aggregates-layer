using Nest;

namespace DoubleGis.Erm.Elastic.Nest.Qds
{
    public interface IElasticResponseHandler
    {
        void ThrowWhenError(IResponse response);
    }
}