using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public partial class AdvertisementElementDomainEntityDto : IDomainEntityDto<AdvertisementElement>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public EntityReference AdvertisementRef { get; set; }

        [DataMember]
        public EntityReference AdvertisementElementTemplateRef { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public long? FileId { get; set; }

        [DataMember]
        public String FileName { get; set; }

        [DataMember]
        public DateTime? BeginDate { get; set; }

        [DataMember]
        public DateTime? EndDate { get; set; }

        [DataMember]
        public FasComment? FasCommentType { get; set; }

        [DataMember]
        public bool IsDeleted { get; set; }

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
        public EntityReference AdsTemplatesAdsElementTemplatesRef { get; set; }

        [DataMember]
        public long? DgppId { get; set; }
    }
}