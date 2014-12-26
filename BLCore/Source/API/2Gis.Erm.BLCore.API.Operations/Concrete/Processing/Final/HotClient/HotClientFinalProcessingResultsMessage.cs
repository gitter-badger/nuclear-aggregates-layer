using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.HotClients;
using DoubleGis.Erm.Platform.API.Core.Messaging;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final.HotClient;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Processing.Final.HotClient
{
    public class HotClientFinalProcessingResultsMessage : MessageBase, IProcessingResultMessage
    {
        private readonly Guid _id = Guid.NewGuid();

        public override Guid Id
        {
            get { return _id; }
        }

        public IMessageFlow TargetFlow
        {
            get { return FinalProcessHotClientPerformedOperationsFlow.Instance; }
        }

        public HotClientRequestDto HotClientRequest { get; set; }
        public long OwnerId { get; set; }
        public RegardingObject RegardingObject { get; set; }
    }
}