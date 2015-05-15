using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class ReleaseInfoDomainEntityDto : IDomainEntityDto<ReleaseInfo>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public DateTime StartDate { get; set; }

        [DataMember]
        public DateTime? FinishDate { get; set; }

        [DataMember]
        public DateTime PeriodStartDate { get; set; }

        [DataMember]
        public DateTime PeriodEndDate { get; set; }

        [DataMember]
        public EntityReference OrganizationUnitRef { get; set; }

        [DataMember]
        public bool IsBeta { get; set; }

        [DataMember]
        public string Comment { get; set; }

        [DataMember]
        public ReleaseStatus Status { get; set; }

        [DataMember]
        public bool IsDeleted { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

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
        public long? FileId { get; set; }

        [DataMember]
        public String FileName { get; set; }
    }
}