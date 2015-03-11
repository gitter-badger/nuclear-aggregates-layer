using Microsoft.Practices.Unity.InterceptionExtension;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.Platform.DI.Interception.PolicyInjection.Handlers
{
    public abstract class LoggingCallHandler : ICallHandler 
    {
        protected readonly ITracer Tracer;

        protected LoggingCallHandler(ITracer tracer)
        {
            Tracer = tracer;
        }

        public int Order { get; set; }

        public abstract IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext);
    }
}