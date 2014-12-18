using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class LimitDomainEntityDto : IDomainEntityDto<Limit>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public Guid ReplicationCode { get; set; }

        [DataMember]
        public EntityReference AccountRef { get; set; }

        [DataMember]
        public DateTime? CloseDate { get; set; }

        [DataMember]
        public decimal Amount { get; set; }

        [DataMember]
        public LimitStatus Status { get; set; }

        [DataMember]
        public DateTime StartPeriodDate { get; set; }

        [DataMember]
        public DateTime EndPeriodDate { get; set; }

        [DataMember]
        public long InspectorCode { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public bool IsDeleted { get; set; }

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
        public byte[] Timestamp { get; set; }

        [DataMember]
        public string Comment { get; set; }

        [DataMember]
        public EntityReference LegalPersonRef { get; set; }

        [DataMember]
        public EntityReference BranchOfficeRef { get; set; }

        [DataMember]
        public EntityReference InspectorRef { get; set; }

        [DataMember]
        public Uri BasicOperationsServiceUrl { get; set; }

        [DataMember]
        public string ErrorMessage { get; set; }
    }
}