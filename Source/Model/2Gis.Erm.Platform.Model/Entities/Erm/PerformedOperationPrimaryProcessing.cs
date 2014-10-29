using System;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class PerformedOperationPrimaryProcessing :
        IEntity
    {
        public Guid UseCaseId { get; set; }
        public Guid MessageFlowId { get; set; }
        public DateTime CreatedOn { get; set; }
        public int AttemptCount { get; set; }
        public DateTime? LastProcessedOn { get; set; }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}