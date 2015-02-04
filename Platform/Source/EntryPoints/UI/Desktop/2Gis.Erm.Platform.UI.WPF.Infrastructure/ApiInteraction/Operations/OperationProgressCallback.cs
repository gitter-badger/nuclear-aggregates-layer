using System;
using System.ServiceModel;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases.Messages;
using DoubleGis.Platform.UI.WPF.Infrastructure.Messaging;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ApiInteraction.Operations
{
    [CallbackBehavior(UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public sealed class OperationProgressCallback : IOperationProgressCallback
    {
        private readonly IMessageSink _messageSink;
        private readonly ICommonLog _logger;

        public OperationProgressCallback(IMessageSink messageSink, ICommonLog logger)
        {
            _messageSink = messageSink;
            _logger = logger;
        }

        public void NotifyAboutProgress(Guid operationToken, IOperationResult[] results)
        {
            _logger.DebugFormat("Callback received. Operation: {0}. Results count: {1}", operationToken, results != null ? results.Length : -1);
            _messageSink.Post(new OperationProgressMessage(operationToken, results));
        }
    }
}
