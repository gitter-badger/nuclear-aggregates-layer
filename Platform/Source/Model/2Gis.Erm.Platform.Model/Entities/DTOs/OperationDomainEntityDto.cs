using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class OperationDomainEntityDto : IDomainEntityDto<Operation>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public Guid Guid { get; set; }

        [DataMember]
        public OperationStatus Status { get; set; }

        [DataMember]
        public long? LogFileId { get; set; }

        [DataMember]
        public String LogFileName { get; set; }

        [DataMember]
        public DateTime? StartTime { get; set; }

        [DataMember]
        public byte[] Timestamp { get; set; }

        [DataMember]
        public DateTime? FinishTime { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public BusinessOperation Type { get; set; }

        [DataMember]
        public EntityReference OwnerRef { get; set; }

        [DataMember]
        public EntityReference CreatedByRef { get; set; }

        [DataMember]
        public EntityReference ModifiedByRef { get; set; }

        [DataMember]
        public DateTime CreatedOn { get; set; }

        [DataMember]
        public DateTime? ModifiedOn { get; set; }

        [DataMember]
        public EntityReference OrganizationUnitRef { get; set; }
    }
}