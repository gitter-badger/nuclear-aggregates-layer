using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class OrderProcessingRequestDomainEntityDto : IDomainEntityDto<OrderProcessingRequest>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public Guid ReplicationCode { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public OrderProcessingRequestType RequestType { get; set; }

        [DataMember]
        public DateTime DueDate { get; set; }

        [DataMember]
        public EntityReference BaseOrderRef { get; set; }

        [DataMember]
        public EntityReference RenewedOrderRef { get; set; }

        [DataMember]
        public EntityReference SourceOrganizationUnitRef { get; set; }

        [DataMember]
        public DateTime BeginDistributionDate { get; set; }

        [DataMember]
        public EntityReference FirmRef { get; set; }

        [DataMember]
        public EntityReference LegalPersonProfileRef { get; set; }

        [DataMember]
        public EntityReference LegalPersonRef { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public OrderProcessingRequestState State { get; set; }

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
        public short ReleaseCountPlan { get; set; }
    }
}