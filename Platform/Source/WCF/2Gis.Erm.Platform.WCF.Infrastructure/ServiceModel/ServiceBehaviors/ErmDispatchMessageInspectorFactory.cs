using System.ServiceModel.Dispatcher;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.ServiceBehaviors
{
    public class ErmDispatchMessageInspectorFactory : IDispatchMessageInspectorFactory
    {
        private readonly ITracerContextManager _tracerContextManager;

        public ErmDispatchMessageInspectorFactory(ITracerContextManager tracerContextManager)
        {
            _tracerContextManager = tracerContextManager;
        }

        public IDispatchMessageInspector Create()
        {
            return new ErmOperationContextMessageInspector(_tracerContextManager);
        }
    }
}