using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class AdvertisementTemplateDomainEntityDto : IDomainEntityDto<AdvertisementTemplate>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Comment { get; set; }

        [DataMember]
        public bool IsDeleted { get; set; }

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
        public bool IsAllowedToWhiteList { get; set; }

        [DataMember]
        public bool IsAdvertisementRequired { get; set; }

        [DataMember]
        public EntityReference DummyAdvertisementRef { get; set; }

        [DataMember]
        public bool IsPublished { get; set; }

        [DataMember]
        public bool HasActiveAdvertisement { get; set; }
    }
}