using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Transports.ServiceBusForWindowsServer;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging.Transports.ServiceBusForWindowsServer;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Core.Messaging.Transports.ServiceBusForWindowsServer;

using Microsoft.ServiceBus.Messaging;

namespace DoubleGis.Erm.BLCore.TaskService.Jobs.PerformedOperationsProcessing.Analysis
{
    public sealed partial class PerformedOperationsConsumerJob
    {
        private interface IPerformedOperationsConsumer
        {
            Task Consume();
        }

        private class PerformedOperationsFlowConsumer<TMessageFlow> : IPerformedOperationsConsumer
            where TMessageFlow : class, IMessageFlow, new()
        {
            private readonly TMessageFlow _targetFlow = new TMessageFlow();
            private readonly int _batchSize;
            private readonly CancellationToken _cancellationToken;
            private readonly ICommonLog _logger;
            private readonly IServiceBusMessageReceiver<TMessageFlow> _messageReceiver;
            private readonly Task _worker;

            public PerformedOperationsFlowConsumer(
                IServiceBusMessageReceiverSettings serviceBusMessageReceiverSettings,
                int batchSize,
                CancellationToken cancellationToken,
                ICommonLog logger)
            {
                _batchSize = batchSize;
                _cancellationToken = cancellationToken;
                _logger = logger;

                _messageReceiver = new ServiceBusMessageReceiver<TMessageFlow>(logger, serviceBusMessageReceiverSettings);
                _worker = new Task(WorkerFunc, TaskCreationOptions.LongRunning);
            }

            Task IPerformedOperationsConsumer.Consume()
            {
                _worker.Start();
                return _worker;
            }

            private void WorkerFunc()
            {
                var stopwatch = new Stopwatch();
                using (_messageReceiver)
                {
                    while (!_cancellationToken.IsCancellationRequested)
                    {
                        stopwatch.Restart();
                        IEnumerable<BrokeredMessage> receivedMessages;

                        try
                        {
                            receivedMessages = _messageReceiver.ReceiveBatch(_batchSize);
                        }
                        catch (Exception ex)
                        {
                            stopwatch.Stop();
                            _logger.ErrorFormatEx(ex, "Consuming performed operations. Can't receive batch from service bus flow {0}, recieve/complete failed after {1}",  _targetFlow, stopwatch.Elapsed.TotalSeconds);
                            continue;
                        }

                        var completedMessages = new List<Guid>();
                        int messageCounter = 0;
                        foreach (var msg in receivedMessages)
                        {
                            ++messageCounter;
                            completedMessages.Add(msg.LockToken);
                        }

                        try
                        {
                            _messageReceiver.CompleteBatch(completedMessages);
                        }
                        catch (Exception ex)
                        {
                            stopwatch.Stop();
                            _logger.ErrorFormatEx(ex, "Consuming performed operations. Can't complete received batch of {0} messages from service bus flow {1}, recieve/complete failed after {2}", messageCounter, _targetFlow, stopwatch.Elapsed.TotalSeconds);
                            continue;
                        }

                        stopwatch.Stop();
                        _logger.DebugFormatEx("Consuming performed operations. Consumed {0} messages from flow {1} and it takes {2:F2} sec, consuming rate {3} op/sec", messageCounter, _targetFlow, stopwatch.Elapsed.TotalSeconds, messageCounter / stopwatch.Elapsed.TotalSeconds);
                        Thread.Sleep(3);
                    }
                }
            }
        }
    }
}
