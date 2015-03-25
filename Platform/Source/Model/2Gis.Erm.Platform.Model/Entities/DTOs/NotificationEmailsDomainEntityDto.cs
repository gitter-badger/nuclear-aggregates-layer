using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class NotificationEmailsDomainEntityDto : IDomainEntityDto<NotificationEmails>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public EntityReference SenderRef { get; set; }

        [DataMember]
        public string Subject { get; set; }

        [DataMember]
        public string SubjectEncoding { get; set; }

        [DataMember]
        public string Body { get; set; }

        [DataMember]
        public string BodyEncoding { get; set; }

        [DataMember]
        public DateTime? ExpirationTime { get; set; }

        [DataMember]
        public string Priority { get; set; }

        [DataMember]
        public int? MaxAttemptsCount { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public bool IsDeleted { get; set; }

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
        public bool IsBodyHtml { get; set; }
    }
}