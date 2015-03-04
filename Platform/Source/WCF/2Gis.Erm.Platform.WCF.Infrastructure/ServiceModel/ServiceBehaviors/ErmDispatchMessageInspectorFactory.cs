using System.ServiceModel.Dispatcher;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.ServiceBehaviors
{
    public class ErmDispatchMessageInspectorFactory : IDispatchMessageInspectorFactory
    {
        private readonly ILoggerContextManager _loggerContextManager;

        public ErmDispatchMessageInspectorFactory(ILoggerContextManager loggerContextManager)
        {
            _loggerContextManager = loggerContextManager;
        }

        public IDispatchMessageInspector Create()
        {
            return new ErmOperationContextMessageInspector(_loggerContextManager);
        }
    }
}