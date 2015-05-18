using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class NoteDomainEntityDto : IDomainEntityDto<Note>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public EntityReference ParentRef { get; set; }

        [DataMember]
        public int ParentType { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public long? FileId { get; set; }

        [DataMember]
        public String FileName { get; set; }

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
        public IEntityType ParentTypeName { get; set; }

        [DataMember]
        public long FileContentLength { get; set; }

        [DataMember]
        public string FileContentType { get; set; }
    }
}