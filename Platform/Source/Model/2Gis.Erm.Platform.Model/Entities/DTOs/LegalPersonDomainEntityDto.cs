using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class LegalPersonDomainEntityDto : IDomainEntityDto<LegalPerson>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public Guid ReplicationCode { get; set; }

        [DataMember]
        public long? DgppId { get; set; }

        [DataMember]
        public bool IsInSyncWith1C { get; set; }

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
        public string Inn { get; set; }

        [DataMember]
        public string Kpp { get; set; }

        [DataMember]
        public string PassportSeries { get; set; }

        [DataMember]
        public string PassportNumber { get; set; }

        [DataMember]
        public string PassportIssuedBy { get; set; }

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
        public string VAT { get; set; }

        [DataMember]
        public string CardNumber { get; set; }

        [DataMember]
        public string Ic { get; set; }

        [DataMember]
        // Это св-во не учавствует в логике и используется только для метаданных.
        public string BusinessmanInn { get; set; }

        [DataMember]
        public bool HasProfiles { get; set; }
    }
}