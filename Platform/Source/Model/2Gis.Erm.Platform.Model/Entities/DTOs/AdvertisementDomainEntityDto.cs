using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class AdvertisementDomainEntityDto : IDomainEntityDto<Advertisement>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public EntityReference FirmRef { get; set; }

        [DataMember]
        public EntityReference AdvertisementTemplateRef { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Comment { get; set; }

        [DataMember]
        public bool IsDeleted { get; set; }

        [DataMember]
        public EntityReference OwnerRef { get; set; }

        [DataMember]
        public DateTime CreatedOn { get; set; }

        [DataMember]
        public DateTime? ModifiedOn { get; set; }

        [DataMember]
        public EntityReference CreatedByRef { get; set; }

        [DataMember]
        public EntityReference ModifiedByRef { get; set; }

        [DataMember]
        public byte[] Timestamp { get; set; }

        [DataMember]
        public bool IsSelectedToWhiteList { get; set; }

        [DataMember]
        public long? DgppId { get; set; }

        [DataMember]
        public bool HasAssignedOrder { get; set; }

        [DataMember]
        public bool IsReadOnlyTemplate { get; set; }

        [DataMember]
        public bool UserDoesntHaveRightsToEditFirm { get; set; }
    }
}