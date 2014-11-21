using System.ServiceModel.Dispatcher;

using DoubleGis.Erm.Platform.Common.Logging;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.Logging
{
    public class ErrorHandlerFactory : IErrorHandlerFactory
    {
        private readonly ICommonLog _logger;

        public ErrorHandlerFactory(ICommonLog logger)
        {
            _logger = logger;
        }

        public IErrorHandler Create()
        {
            return new Log4NetErrorHandler(_logger);
        }
    }
}