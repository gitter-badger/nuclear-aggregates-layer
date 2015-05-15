using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class HotClientRequestDomainEntityDto : IDomainEntityDto<HotClientRequest>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string SourceCode { get; set; }

        [DataMember]
        public string UserCode { get; set; }

        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public DateTime CreationDate { get; set; }

        [DataMember]
        public string ContactName { get; set; }

        [DataMember]
        public string ContactPhone { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public long? CardCode { get; set; }

        [DataMember]
        public long? BranchCode { get; set; }

        [DataMember]
        public EntityReference TaskRef { get; set; }

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
    }
}