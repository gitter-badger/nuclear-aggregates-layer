using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class BillDomainEntityDto : IDomainEntityDto<Bill>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public long? DgppId { get; set; }

        [DataMember]
        public long OrderId { get; set; }

        [DataMember]
        public string Number { get; set; }

        [DataMember]
        public DateTime BillDate { get; set; }

        [DataMember]
        public DateTime BeginDistributionDate { get; set; }

        [DataMember]
        public DateTime EndDistributionDate { get; set; }

        [DataMember]
        public DateTime PaymentDatePlan { get; set; }

        [DataMember]
        public decimal PayablePlan { get; set; }

        [DataMember]
        public decimal VatPlan { get; set; }

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
        public bool IsOrderActive { get; set; }
    }
}