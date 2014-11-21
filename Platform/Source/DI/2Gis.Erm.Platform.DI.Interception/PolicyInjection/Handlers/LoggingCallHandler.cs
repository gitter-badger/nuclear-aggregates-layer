using DoubleGis.Erm.Platform.Common.Logging;

using Microsoft.Practices.Unity.InterceptionExtension;

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