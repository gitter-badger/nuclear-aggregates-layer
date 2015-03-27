using System;
using System.ServiceModel;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Messages;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Operations
{
    [CallbackBehavior(UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public sealed class OperationProgressCallback : IOperationProgressCallback
    {
        private readonly IMessageSink _messageSink;
        private readonly ITracer _tracer;

        public OperationProgressCallback(IMessageSink messageSink, ITracer tracer)
        {
            _messageSink = messageSink;
            _tracer = tracer;
        }

        public void NotifyAboutProgress(Guid operationToken, IOperationResult[] results)
        {
            _tracer.DebugFormat("Callback received. Operation: {0}. Results count: {1}", operationToken, results != null ? results.Length : -1);
            _messageSink.Post(new OperationProgressMessage(operationToken, results));
        }
    }
}
