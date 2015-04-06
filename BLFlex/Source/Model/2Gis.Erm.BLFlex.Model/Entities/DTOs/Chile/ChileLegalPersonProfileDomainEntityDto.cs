using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Chile
{
    // FIXME {all, 14.02.2014}: Временное решение, до начала работы IPartable на сущностях IDomainEntityDto
    // COMMENT {a.rechkalov, 21.04.2014}: Что скрывается за этой фразой?
    // COMMENT {a.rechkalov, 04.12.2014}: Теперь понял.
    [DataContract]
    public sealed class ChileLegalPersonProfileDomainEntityDto : IDomainEntityDto<DoubleGis.Erm.Platform.Model.Entities.Erm.LegalPersonProfile>, IChileAdapted
    {
        [DataMember]
        public long Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public EntityReference LegalPersonRef { get; set; }
        [DataMember]
        public bool IsMainProfile { get; set; }
        [DataMember]
        public string PositionInNominative { get; set; }
        [DataMember]
        public string PositionInGenitive { get; set; }
        [DataMember]
        public string ChiefNameInNominative { get; set; }
        [DataMember]
        public string ChiefNameInGenitive { get; set; }
        [DataMember]
        public OperatesOnTheBasisType OperatesOnTheBasisInGenitive { get; set; }
        [DataMember]
        public string DocumentsDeliveryAddress { get; set; }
        [DataMember]
        public string PostAddress { get; set; }
        [DataMember]
        public string RecipientName { get; set; }
        [DataMember]
        public DocumentsDeliveryMethod DocumentsDeliveryMethod { get; set; }
        [DataMember]
        public string EmailForAccountingDocuments { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string PersonResponsibleForDocuments { get; set; }
        [DataMember]
        public string Phone { get; set; }
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
        public PaymentMethod PaymentMethod { get; set; }
        [DataMember]
        public string AccountNumber { get; set; }
        [DataMember]
        public string PaymentEssentialElements { get; set; }
        [DataMember]
        public AccountType AccountType { get; set; }
        [DataMember]
        public EntityReference BankRef { get; set; }
        [DataMember]
        public string RepresentativeRut { get; set; }
        [DataMember]
        public DateTime? RepresentativeDocumentIssuedOn { get; set; }
        [DataMember]
        public string RepresentativeDocumentIssuedBy { get; set; }
        [DataMember]
        public LegalPersonType LegalPersonType { get; set; }
    }
}
