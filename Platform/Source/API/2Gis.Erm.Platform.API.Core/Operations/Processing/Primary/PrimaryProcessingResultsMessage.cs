using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Messaging;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Messaging.Processing;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary
{
    public sealed class PrimaryProcessingResultsMessage : MessageBase, IProcessingResultMessage
    {
        private readonly Guid _id = Guid.NewGuid();

        public override Guid Id
        {
            get { return _id; }
        }

        public IMessageFlow TargetFlow { get; set; }
        public IEnumerable<PerformedOperationFinalProcessing> Results { get; set; }
    }
}
