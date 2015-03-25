using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class AdvertisementElementDenialReasonDomainEntityDto : IDomainEntityDto<AdvertisementElementDenialReason>
    {
        [DataMember]
        public EntityReference AdvertisementElementRef { get; set; }

        [DataMember]
        public EntityReference DenialReasonRef { get; set; }

        [DataMember]
        public string Comment { get; set; }

        [DataMember]
        public EntityReference CreatedByRef { get; set; }

        [DataMember]
        public DateTime CreatedOn { get; set; }

        [DataMember]
        public EntityReference ModifiedByRef { get; set; }

        [DataMember]
        public DateTime? ModifiedOn { get; set; }

        [DataMember]
        public byte[] Timestamp { get; set; }

        [DataMember]
        public long Id { get; set; }
    }
}