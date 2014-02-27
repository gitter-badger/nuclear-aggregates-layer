using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLFlex.Model.Entities.DTOs
{
    // FIXME {all, 14.02.2014}: Временное решение, до начала работы IPartable на сущностях IDomainEntityDto
    [DataContract]
    public sealed class ChileLegalPersonProfileDomainEntityDto : IDomainEntityDto<LegalPersonProfile>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public EntityReference LegalPersonRef { get; set; }
        public string DocumentsDeliveryAddress { get; set; }
        public string RecipientName { get; set; }
        public string PersonResponsibleForDocuments { get; set; }
        public DocumentsDeliveryMethod DocumentsDeliveryMethod { get; set; }
        public string EmailForAccountingDocuments { get; set; }
        public string AdditionalEmail { get; set; }
        public string PostAddress { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public AccountType AccountType { get; set; }
        public string AccountNumber { get; set; }
        public EntityReference BankRef { get; set; }
        public string AdditionalPaymentElements { get; set; }
        public string RepresentativeName { get; set; }
        public string RepresentativePosition { get; set; }
        public string RepresentativeRut { get; set; }
        public string Phone { get; set; }
        public OperatesOnTheBasisType OperatesOnTheBasisInGenitive { get; set; }
        public DateTime? RepresentativeDocumentIssuedOn { get; set; }
        public string RepresentativeDocumentIssuedBy { get; set; }
        public bool IsMainProfile { get; set; }
        public EntityReference OwnerRef { get; set; }
        public EntityReference CreatedByRef { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public EntityReference ModifiedByRef { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }
    }
}
