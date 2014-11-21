using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Messaging;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary.ElasticSearch
{
    public sealed class ReplicateToElasticSearchPrimaryProcessingResultsMessage : MessageBase, IProcessingResultMessage
    {
        private readonly Guid _id = Guid.NewGuid();

        public override Guid Id
        {
            get { return _id; }
        }

        public IMessageFlow TargetFlow
        {
            get { return PrimaryReplicate2ElasticSearchPerformedOperationsFlow.Instance; }
        }

        public IReadOnlyCollection<EntityLink> EntityLinks { get; set; }
    }

    public sealed class EntityLink
    {
        public Type EntityType { get; set; }
        public IReadOnlyCollection<long> UpdatedIds { get; set; }
    }
}