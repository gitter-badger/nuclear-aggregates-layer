using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DoubleGis.Erm.Platform.API.Core.Messaging;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Receivers;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.Platform.TaskService.Jobs.Concrete.PerformedOperationsProcessing.Analysis.Consumer
{
    public sealed class PerformedOperationsFlowConsumer<TMessageFlow> : IPerformedOperationsConsumer
            where TMessageFlow : class, IMessageFlow, new()
    {
        private const int MaxDelayMs = 30000;
        private const int DelayIncrementMs = 2000;
        private const decimal MinEffectiveBatchRatio = 0.1M;

        private readonly TMessageFlow _targetFlow = new TMessageFlow();
        private readonly IPerformedOperationsReceiverSettings _performedOperationsReceiverSettings;
        private readonly IMessageReceiver _performedOperationMessagesReceiver;
        private readonly CancellationToken _cancellationToken;
        private readonly ITracer _tracer;
        private readonly Task _worker;

        public PerformedOperationsFlowConsumer(
            IPerformedOperationsReceiverSettings performedOperationsReceiverSettings, 
            IMessageReceiver performedOperationMessagesReceiver, 
            CancellationToken cancellationToken, 
            ITracer tracer)
        {
            _performedOperationsReceiverSettings = performedOperationsReceiverSettings;
            _performedOperationMessagesReceiver = performedOperationMessagesReceiver;
            _cancellationToken = cancellationToken;
            _tracer = tracer;

            _worker = new Task(WorkerFunc, TaskCreationOptions.LongRunning);
        }

        Task IPerformedOperationsConsumer.Consume()
        {
            _worker.Start();
            return _worker;
        }

        private void WorkerFunc()
        {
            _tracer.InfoFormat("Consuming performed operations. Consumer for flow {0} started", _targetFlow);

            var stopwatch = new Stopwatch();

            try
            {
                int actualDealyMs = 0;

                while (!_cancellationToken.IsCancellationRequested)
                {
                    stopwatch.Restart();
                    IReadOnlyList<IMessage> receivedMessages = null;

                    bool isConsumingAttemptFailed = false;

                    try
                    {
                        receivedMessages = _performedOperationMessagesReceiver.Peek();
                    }
                    catch (Exception ex)
                    {
                        stopwatch.Stop();
                        _tracer.ErrorFormat(ex, "Consuming performed operations. Can't receive batch from service bus flow {0}, recieve/complete failed after {1}",  _targetFlow, stopwatch.Elapsed.TotalSeconds);

                        isConsumingAttemptFailed = true;
                        actualDealyMs = Math.Min(MaxDelayMs, actualDealyMs + (DelayIncrementMs * 5));
                    }

                    int receivedMessageCount = receivedMessages != null ? receivedMessages.Count : 0;
                    if (!isConsumingAttemptFailed && receivedMessageCount > 0)
                    {
                        try
                        {
                            _performedOperationMessagesReceiver.Complete(receivedMessages, Enumerable.Empty<IMessage>());
                        }
                        catch (Exception ex)
                        {
                            stopwatch.Stop();
                            _tracer.ErrorFormat(ex, "Consuming performed operations. Can't complete received batch of {0} messages from service bus flow {1}, recieve/complete failed after {2}", receivedMessageCount, _targetFlow, stopwatch.Elapsed.TotalSeconds);

                            isConsumingAttemptFailed = true;
                            actualDealyMs = Math.Min(MaxDelayMs, actualDealyMs + (DelayIncrementMs * 5));
                        }
                    }

                    if (!isConsumingAttemptFailed)
                    {
                        stopwatch.Stop();
                        _tracer.InfoFormat("Consuming performed operations. Consumed {0} messages from flow {1} and it takes {2:F2} sec, consuming rate {3} op/sec",
                                              receivedMessageCount,
                                              _targetFlow,
                                              stopwatch.Elapsed.TotalSeconds,
                                              receivedMessageCount / stopwatch.Elapsed.TotalSeconds);

                        if ((decimal)receivedMessageCount / _performedOperationsReceiverSettings.BatchSize > MinEffectiveBatchRatio)
                        {
                            actualDealyMs = 0;
                            continue;
                        }

                        actualDealyMs = Math.Min(MaxDelayMs, actualDealyMs + DelayIncrementMs);
                    }

                    _tracer.InfoFormat("Consuming performed operations. Consuming dealy applied, actual dealy ms: {0}. Consuming failed: {1}. Target flow {2}",
                                         actualDealyMs,
                                         isConsumingAttemptFailed,
                                         _targetFlow);

                    Thread.Sleep(actualDealyMs);
                }
            }
            finally
            {
                var disposablePerformedOperationMessagesReceiver = _performedOperationMessagesReceiver as IDisposable;
                if (disposablePerformedOperationMessagesReceiver != null)
                {
                    disposablePerformedOperationMessagesReceiver.Dispose();
                }
            }

            _tracer.InfoFormat("Consuming performed operations. Consumer for flow {0} stopped", _targetFlow);
        }
    }
}
