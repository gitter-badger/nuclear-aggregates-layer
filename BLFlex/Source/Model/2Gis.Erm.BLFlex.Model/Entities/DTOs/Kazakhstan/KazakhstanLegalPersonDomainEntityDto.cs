using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Kazakhstan
{
    [DataContract]
    public class KazakhstanLegalPersonDomainEntityDto : IDomainEntityDto<LegalPerson>, IKazakhstanAdapted
    {
        [DataMember]
        public long Id { get; set; }
        [DataMember]
        public Guid ReplicationCode { get; set; }
        [DataMember]
        public long? DgppId { get; set; }
        [DataMember]
        public EntityReference ClientRef { get; set; }
        [DataMember]
        public string LegalName { get; set; }
        [DataMember]
        public string ShortName { get; set; }
        [DataMember]
        public LegalPersonType LegalPersonTypeEnum { get; set; }
        [DataMember]
        public string LegalAddress { get; set; }
        [DataMember]
        public string Bin { get; set; }
        [DataMember]
        public string IdentityCardNumber { get; set; }
        [DataMember]
        public string IdentityCardIssuedBy { get; set; }
        [DataMember]
        public DateTime? IdentityCardIssuedOn { get; set; }
        [DataMember]
        public string RegistrationAddress { get; set; }
        [DataMember]
        public string Comment { get; set; }
        [DataMember]
        public bool IsDeleted { get; set; }
        [DataMember]
        public bool IsActive { get; set; }
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
        public bool HasProfiles { get; set; }
    }
}
