using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class DeniedPositionDomainEntityDto : IDomainEntityDto<DeniedPosition>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public EntityReference PositionRef { get; set; }

        [DataMember]
        public EntityReference PositionDeniedRef { get; set; }

        [DataMember]
        public EntityReference PriceRef { get; set; }

        [DataMember]
        public bool IsDeleted { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public ObjectBindingType ObjectBindingType { get; set; }

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
        public bool PriceIsPublished { get; set; }
    }
}