using System.ServiceModel.Dispatcher;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.Logging
{
    public class ErrorHandlerFactory : IErrorHandlerFactory
    {
        private readonly ITracer _tracer;

        public ErrorHandlerFactory(ITracer tracer)
        {
            _tracer = tracer;
        }

        public IErrorHandler Create()
        {
            return new Log4NetErrorHandler(_tracer);
        }
    }
}