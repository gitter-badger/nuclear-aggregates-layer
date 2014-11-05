using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Messaging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.ReadModel.DTOs
{
    public sealed class DBPerformedOperationsMessage : MessageBase
    {
        public override Guid Id
        {
            get { return TargetUseCase.UseCaseId; }
        }

        public IEnumerable<PerformedBusinessOperation> Operations { get; set; }
        public PerformedOperationPrimaryProcessing TargetUseCase { get; set; }
    }
}
