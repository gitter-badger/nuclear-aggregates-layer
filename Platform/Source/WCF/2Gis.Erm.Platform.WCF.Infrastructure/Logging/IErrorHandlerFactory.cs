using System.ServiceModel.Dispatcher;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.Logging
{
    public interface IErrorHandlerFactory
    {
        IErrorHandler Create();
    }
}