using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.MsCRM.Dto;
using DoubleGis.Erm.Platform.API.Core.Messaging;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final.HotClient;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Processing.Final.HotClient
{
    public class ReplicateHotClientFinalProcessingResultsMessage : MessageBase, IProcessingResultMessage
    {
        private readonly Guid _id = Guid.NewGuid();

        public override Guid Id
        {
            get { return _id; }
        }

        public IMessageFlow TargetFlow
        {
            get { return FinalReplicateHotClientPerformedOperationsFlow.Instance; }
        }

        public UserDto Owner { get; set; }
        public HotClientRequestDto HotClient { get; set; }
        public RegardingObject RegardingObject { get; set; }
        public HotClientRequest RequestEntity { get; set; }
    }
}