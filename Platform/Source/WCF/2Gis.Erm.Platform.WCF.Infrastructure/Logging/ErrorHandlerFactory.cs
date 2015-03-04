using System.ServiceModel.Dispatcher;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.Logging
{
    public class ErrorHandlerFactory : IErrorHandlerFactory
    {
        private readonly ITracer _logger;

        public ErrorHandlerFactory(ITracer logger)
        {
            _logger = logger;
        }

        public IErrorHandler Create()
        {
            return new Log4NetErrorHandler(_logger);
        }
    }
}