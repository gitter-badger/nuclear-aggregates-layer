using Microsoft.Practices.Unity.InterceptionExtension;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.Platform.DI.Interception.PolicyInjection.Handlers
{
    public abstract class LoggingCallHandler : ICallHandler 
    {
        protected readonly ICommonLog Logger;

        protected LoggingCallHandler(ICommonLog logger)
        {
            Logger = logger;
        }

        public int Order { get; set; }

        public abstract IMethodReturn Invoke(IMethodInvocation input, GetNextHandlerDelegate getNext);
    }
}