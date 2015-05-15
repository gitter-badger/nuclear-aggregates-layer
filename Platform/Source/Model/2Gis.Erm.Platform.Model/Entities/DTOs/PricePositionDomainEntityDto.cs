using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class PricePositionDomainEntityDto : IDomainEntityDto<PricePosition>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public long? DgppId { get; set; }

        [DataMember]
        public EntityReference PriceRef { get; set; }

        [DataMember]
        public EntityReference PositionRef { get; set; }

        [DataMember]
        public decimal Cost { get; set; }

        [DataMember]
        public int? Amount { get; set; }

        [DataMember]
        public PricePositionAmountSpecificationMode AmountSpecificationMode { get; set; }

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
        public int? MinAdvertisementAmount { get; set; }

        [DataMember]
        public int? MaxAdvertisementAmount { get; set; }

        [DataMember]
        public PricePositionRateType RateType { get; set; }

        [DataMember]
        public bool IsRateTypeAvailable { get; set; }

        [DataMember]
        public bool IsPositionControlledByAmount { get; set; }

        [DataMember]
        public bool PriceIsDeleted { get; set; }

        [DataMember]
        public bool PriceIsPublished { get; set; }

        [DataMember]
        public EntityReference CurrencyRef { get; set; }
    }
}