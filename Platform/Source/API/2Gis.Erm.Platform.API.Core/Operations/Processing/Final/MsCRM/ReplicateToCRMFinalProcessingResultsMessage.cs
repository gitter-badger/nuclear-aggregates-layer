using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Messaging;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final.MsCRM
{
    public sealed class ReplicateToCRMFinalProcessingResultsMessage : MessageBase, IProcessingResultMessage
    {
        private readonly Guid _id = Guid.NewGuid();

        public override Guid Id
        {
            get { return _id; }
        }

        public IMessageFlow TargetFlow
        {
            get { return FinalReplicate2MsCRMPerformedOperationsFlow.Instance; }
        }

        public Type EntityType { get; set; }
        public IEnumerable<long> Ids { get; set; }
    }
}