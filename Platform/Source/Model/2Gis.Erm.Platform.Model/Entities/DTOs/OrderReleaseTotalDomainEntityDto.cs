using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class OrderReleaseTotalDomainEntityDto : IDomainEntityDto<OrderReleaseTotal>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public long OrderId { get; set; }

        [DataMember]
        public int ReleaseNumber { get; set; }

        [DataMember]
        public DateTime ReleaseBeginDate { get; set; }

        [DataMember]
        public DateTime ReleaseEndDate { get; set; }

        [DataMember]
        public decimal AmountToWithdraw { get; set; }

        [DataMember]
        public EntityReference OwnerRef { get; set; }

        [DataMember]
        public EntityReference CreatedByRef { get; set; }

        [DataMember]
        public DateTime CreatedOn { get; set; }

        [DataMember]
        public EntityReference ModifiedByRef { get; set; }

        [DataMember]
        public DateTime? ModifiedOn { get; set; }

        [DataMember]
        public decimal Vat { get; set; }
    }
}