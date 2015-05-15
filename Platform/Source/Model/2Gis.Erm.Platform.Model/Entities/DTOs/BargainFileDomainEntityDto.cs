using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class BargainFileDomainEntityDto : IDomainEntityDto<BargainFile>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public EntityReference BargainRef { get; set; }

        [DataMember]
        public long FileId { get; set; }

        [DataMember]
        public String FileName { get; set; }

        [DataMember]
        public string Comment { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

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
        public BargainFileKind FileKind { get; set; }

        [DataMember]
        public string FileContentType { get; set; }

        [DataMember]
        public long FileContentLength { get; set; }
    }
}