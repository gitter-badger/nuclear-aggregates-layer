using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class OrderPositionDomainEntityDto : IDomainEntityDto<OrderPosition>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public long OrderId { get; set; }

        [DataMember]
        public long? DgppId { get; set; }

        [DataMember]
        public Guid ReplicationCode { get; set; }

        [DataMember]
        public EntityReference PricePositionRef { get; set; }

        [DataMember]
        public int Amount { get; set; }

        [DataMember]
        public decimal PricePerUnit { get; set; }

        [DataMember]
        public decimal PricePerUnitWithVat { get; set; }

        [DataMember]
        public decimal DiscountSum { get; set; }

        [DataMember]
        public decimal DiscountPercent { get; set; }

        [DataMember]
        public bool CalculateDiscountViaPercent { get; set; }

        [DataMember]
        public decimal PayablePrice { get; set; }

        [DataMember]
        public decimal PayablePlanWoVat { get; set; }

        [DataMember]
        public decimal PayablePlan { get; set; }

        [DataMember]
        public int ShipmentPlan { get; set; }

        [DataMember]
        public string Comment { get; set; }

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
        public decimal CategoryRate { get; set; }

        [DataMember]
        public string OrderNumber { get; set; }

        [DataMember]
        public bool IsComposite { get; set; }

        [DataMember]
        public long OrganizationUnitId { get; set; }

        [DataMember]
        public DateTime PeriodStartDate { get; set; }

        [DataMember]
        public DateTime PeriodEndDate { get; set; }

        [DataMember]
        public long PriceId { get; set; }

        [DataMember]
        public long? OrderFirmId { get; set; }

        [DataMember]
        public long? RequiredPlatformId { get; set; }

        [DataMember]
        public bool IsBlockedByRelease { get; set; }

        [DataMember]
        public int OrderWorkflowStepId { get; set; }

        [DataMember]
        public IEnumerable<AdvertisementDescriptor> Advertisements { get; set; }

        [DataMember]
        public bool IsRated { get; set; }
    }
}