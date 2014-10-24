using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Transports.ServiceBusForWindowsServer;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging.Transports.ServiceBusForWindowsServer;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Core.Messaging.Transports.ServiceBusForWindowsServer;
using DoubleGis.Erm.Platform.TaskService.Jobs;

using Quartz;

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
            private readonly int _batchSize;
            private readonly CancellationToken _cancellationToken;
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

                _messageReceiver = new ServiceBusMessageReceiver<TMessageFlow>(logger, serviceBusMessageReceiverSettings);
                _worker = new Task(WorkerFunc);
            }

            Task IPerformedOperationsConsumer.Consume()
            {
                return _worker;
            }

            private void WorkerFunc()
            {
                using (_messageReceiver)
                {
                    while (!_cancellationToken.IsCancellationRequested)
                    {
                        var receivedMessages = _messageReceiver.ReceiveBatch(_batchSize);
                        
                        var completedMessages = new List<Guid>();
                        int messageCounter = 0;

                        foreach (var msg in receivedMessages)
                        {
                            ++messageCounter;
                            completedMessages.Add(msg.LockToken);
                        }

                        _messageReceiver.CompleteBatch(completedMessages);

                        Thread.Sleep(3);
                    }
                }
            }
        }
    }
}
