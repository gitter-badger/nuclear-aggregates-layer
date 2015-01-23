using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Emirates
{
    [DataContract]
    public sealed class EmiratesLegalPersonProfileDomainEntityDto : IDomainEntityDto<Platform.Model.Entities.Erm.LegalPersonProfile>, IEmiratesAdapted
    {
        [DataMember]
        public long Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public EntityReference LegalPersonRef { get; set; }
        [DataMember]
        public string DocumentsDeliveryAddress { get; set; }
        [DataMember]
        public string RecipientName { get; set; }
        [DataMember]
        public string PersonResponsibleForDocuments { get; set; }
        [DataMember]
        public DocumentsDeliveryMethod DocumentsDeliveryMethod { get; set; }
        [DataMember]
        public string EmailForAccountingDocuments { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string PostAddress { get; set; }
        [DataMember]
        public string Phone { get; set; }
        [DataMember]
        public PaymentMethod PaymentMethod { get; set; }
        [DataMember]
        public string IBAN { get; set; }
        [DataMember]
        public string SWIFT { get; set; }
        [DataMember]
        public string BankName { get; set; }
        [DataMember]
        public string PaymentEssentialElements { get; set; }
        [DataMember]
        public string ChiefNameInNominative { get; set; }
        [DataMember]
        public string PositionInNominative { get; set; }

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
        public bool IsMainProfile { get; set; }

        [DataMember]
        public string Fax { get; set; }
    }
}
