using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Messaging;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Processing.Final
{
    public sealed class PerformedOperationsFinalProcessingMessage : MessageBase
    {
        private readonly Guid _id;

        public PerformedOperationsFinalProcessingMessage()
        {
            _id = Guid.NewGuid();
            FinalProcessings = Enumerable.Empty<PerformedOperationFinalProcessing>();
        }

        public override Guid Id
        {
            get { return _id; }
        }

        public long EntityId { get; set; }
        public IEntityType EntityName { get; set; }
        public int MaxAttemptCount { get; set; }
        public IMessageFlow Flow { get; set; }
        public IEnumerable<PerformedOperationFinalProcessing> FinalProcessings { get; set; }
    }
}