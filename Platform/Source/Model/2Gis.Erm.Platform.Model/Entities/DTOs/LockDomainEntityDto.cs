using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class LockDomainEntityDto : IDomainEntityDto<Lock>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public EntityReference AccountRef { get; set; }

        [DataMember]
        public long OrderId { get; set; }

        [DataMember]
        public DateTime PeriodStartDate { get; set; }

        [DataMember]
        public DateTime PeriodEndDate { get; set; }

        [DataMember]
        public DateTime? CloseDate { get; set; }

        [DataMember]
        public decimal PlannedAmount { get; set; }

        [DataMember]
        public decimal Balance { get; set; }

        [DataMember]
        public decimal? ClosedBalance { get; set; }

        [DataMember]
        public EntityReference DebitAccountDetailRef { get; set; }

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
        public EntityReference BranchOfficeOrganizationUnitRef { get; set; }

        [DataMember]
        public EntityReference LegalPersonRef { get; set; }

        [DataMember]
        public EntityReference OrderRef { get; set; }
    }
}