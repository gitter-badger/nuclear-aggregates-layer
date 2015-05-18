using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class OrderPositionAdvertisementDomainEntityDto : IDomainEntityDto<OrderPositionAdvertisement>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public EntityReference OrderPositionRef { get; set; }

        [DataMember]
        public EntityReference PositionRef { get; set; }

        [DataMember]
        public EntityReference AdvertisementRef { get; set; }

        [DataMember]
        public EntityReference FirmAddressRef { get; set; }

        [DataMember]
        public EntityReference CategoryRef { get; set; }

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
        public EntityReference ThemeRef { get; set; }
    }
}