using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public partial class LocalMessageDomainEntityDto : IDomainEntityDto<LocalMessage>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public DateTime EventDate { get; set; }

        [DataMember]
        public long FileId { get; set; }

        [DataMember]
        public String FileName { get; set; }

        [DataMember]
        public LocalMessageStatus Status { get; set; }

        [DataMember]
        public string ProcessResult { get; set; }

        [DataMember]
        public EntityReference MessageTypeRef { get; set; }

        [DataMember]
        public EntityReference OrganizationUnitRef { get; set; }

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
        public long? ProcessingTime { get; set; }
    }
}