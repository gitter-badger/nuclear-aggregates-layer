using System;
using System.Linq;
using System.Threading;

using DoubleGis.Erm.Platform.API.Core.Messaging;
using DoubleGis.Erm.Platform.API.Core.Messaging.Receivers;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary.MsCRM;
using DoubleGis.Erm.Platform.Common.Prerequisites;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Platform.Operations.Logging;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Platform.Operations.Processing
{
    [Prerequisites(typeof(ServiceBusLoggingTest))]
    public sealed class ServiceBusReceiverTest : IIntegrationTest
    {
        private readonly IMessageReceiverFactory _messageReceiverFactory;

        public ServiceBusReceiverTest(IMessageReceiverFactory messageReceiverFactory)
        {
            _messageReceiverFactory = messageReceiverFactory;
        }

        public ITestResult Execute()
        {
            return Result
                .When(Receive)
                .Then(result => result.Status == TestResultStatus.Succeeded);
        }

        private void Receive()
        {
            var receiver = 
                _messageReceiverFactory.Create<PrimaryReplicate2MsCRMPerformedOperationsFlow, PerformedOperationsReceiverSettings>(
                                            new PerformedOperationsReceiverSettings { BatchSize = 10 });
                
            var messages = receiver.Peek();
                
            Thread.Sleep(10000);
                
            receiver.Complete(messages, Enumerable.Empty<IMessage>());

            var disposableReceiver = receiver as IDisposable;
            if (disposableReceiver != null)
            {
                disposableReceiver.Dispose();
            }
        }
    }
}
