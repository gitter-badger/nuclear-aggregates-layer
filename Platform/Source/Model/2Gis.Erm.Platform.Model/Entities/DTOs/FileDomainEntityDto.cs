using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class FileDomainEntityDto : IDomainEntityDto<File>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public string ContentType { get; set; }

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
        public long ContentLength { get; set; }

        [DataMember]
        public long? DgppId { get; set; }
    }
}