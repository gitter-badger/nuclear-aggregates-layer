using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class PerformedOperationFinalProcessingDomainEntityDto : IDomainEntityDto<PerformedOperationFinalProcessing>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public EntityReference EntityTypeRef { get; set; }

        [DataMember]
        public EntityReference EntityRef { get; set; }

        [DataMember]
        public int AttemptCount { get; set; }

        [DataMember]
        public DateTime CreatedOn { get; set; }

        [DataMember]
        public EntityReference MessageFlowRef { get; set; }

        [DataMember]
        public string Context { get; set; }

        [DataMember]
        public EntityReference OperationRef { get; set; }
    }
}