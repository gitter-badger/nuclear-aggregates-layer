using System;

namespace DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.ReadModel.DTOs
{
    public sealed class PrimaryProcessingFlowStateDto
    {
        public DateTime OldestProcessingTargetCreatedDate { get; set; }
        public int ProcessingTargetsCount { get; set; }
    }
}
