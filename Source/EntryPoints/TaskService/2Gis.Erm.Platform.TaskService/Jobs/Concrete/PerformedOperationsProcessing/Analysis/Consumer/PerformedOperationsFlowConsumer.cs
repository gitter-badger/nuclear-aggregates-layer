using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DoubleGis.Erm.Platform.API.Core.Messaging;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Receivers;
using DoubleGis.Erm.Platform.Common.Logging;

namespace DoubleGis.Erm.Platform.TaskService.Jobs.Concrete.PerformedOperationsProcessing.Analysis.Consumer
{
    public sealed class PerformedOperationsFlowConsumer<TMessageFlow> : IPerformedOperationsConsumer
            where TMessageFlow : class, IMessageFlow, new()
    {
        private readonly TMessageFlow _targetFlow = new TMessageFlow();
        private readonly IMessageReceiver _performedOperationMessagesReceiver;
        private readonly CancellationToken _cancellationToken;
        private readonly ICommonLog _logger;
        private readonly Task _worker;

        public PerformedOperationsFlowConsumer(IMessageReceiver performedOperationMessagesReceiver, CancellationToken cancellationToken, ICommonLog logger)
        {
            _performedOperationMessagesReceiver = performedOperationMessagesReceiver;
            _cancellationToken = cancellationToken;
            _logger = logger;

            _worker = new Task(WorkerFunc, TaskCreationOptions.LongRunning);
        }

        Task IPerformedOperationsConsumer.Consume()
        {
            _worker.Start();
            return _worker;
        }

        private void WorkerFunc()
        {
            _logger.InfoFormatEx("Consuming performed operations. Consumer for flow {0} started", _targetFlow);

            var stopwatch = new Stopwatch();

            try
            {
                while (!_cancellationToken.IsCancellationRequested)
                {
                    stopwatch.Restart();
                    IReadOnlyList<IMessage> receivedMessages;

                    try
                    {
                        receivedMessages = _performedOperationMessagesReceiver.Peek();
                    }
                    catch (Exception ex)
                    {
                        stopwatch.Stop();
                        _logger.ErrorFormatEx(ex, "Consuming performed operations. Can't receive batch from service bus flow {0}, recieve/complete failed after {1}",  _targetFlow, stopwatch.Elapsed.TotalSeconds);
                        continue;
                    }

                    try
                    {
                        _performedOperationMessagesReceiver.Complete(receivedMessages, Enumerable.Empty<IMessage>());
                    }
                    catch (Exception ex)
                    {
                        stopwatch.Stop();
                        _logger.ErrorFormatEx(ex, "Consuming performed operations. Can't complete received batch of {0} messages from service bus flow {1}, recieve/complete failed after {2}", receivedMessages.Count, _targetFlow, stopwatch.Elapsed.TotalSeconds);
                        continue;
                    }

                    stopwatch.Stop();
                    _logger.DebugFormatEx("Consuming performed operations. Consumed {0} messages from flow {1} and it takes {2:F2} sec, consuming rate {3} op/sec", receivedMessages.Count, _targetFlow, stopwatch.Elapsed.TotalSeconds, receivedMessages.Count / stopwatch.Elapsed.TotalSeconds);
                    
                    Thread.Sleep(3);
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

            _logger.InfoFormatEx("Consuming performed operations. Consumer for flow {0} stopped", _targetFlow);
        }
    }
}
