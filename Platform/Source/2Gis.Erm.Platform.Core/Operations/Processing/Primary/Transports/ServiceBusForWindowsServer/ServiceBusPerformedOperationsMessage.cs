using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Messaging;
using DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.ServiceBusForWindowsServer;

using Microsoft.ServiceBus.Messaging;

namespace DoubleGis.Erm.Platform.Core.Operations.Processing.Primary.Transports.ServiceBusForWindowsServer
{
    public sealed class ServiceBusPerformedOperationsMessage : MessageBase
    {
        private readonly Guid _id;
        private readonly IEnumerable<BrokeredMessage> _operations;

        public ServiceBusPerformedOperationsMessage(IEnumerable<BrokeredMessage> operations)
        {
            _id = (Guid)operations.First().Properties[TrackedUseCaseMessageProperties.Names.UseCaseId];
            _operations = operations;
        }

        public override Guid Id
        {
            get { return _id; }
        }

        public IEnumerable<BrokeredMessage> Operations
        {
            get { return _operations; }
        }
    }
}