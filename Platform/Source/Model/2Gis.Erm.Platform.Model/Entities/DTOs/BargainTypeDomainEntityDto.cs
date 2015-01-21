using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class BargainTypeDomainEntityDto : IDomainEntityDto<BargainType>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string SyncCode1C { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public decimal VatRate { get; set; }

        [DataMember]
        public bool IsDeleted { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

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
        public Uri IdentityServiceUrl { get; set; }
    }
}