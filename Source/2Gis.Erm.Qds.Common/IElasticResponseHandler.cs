using Nest;

namespace DoubleGis.Erm.Qds.Common
{
    public interface IElasticResponseHandler
    {
        void ThrowWhenError(IResponse response);
    }
}