using System;
using System.ComponentModel.DataAnnotations;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Kazakhstan;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Kazakhstan
{
    public sealed class KazakhstanLegalPersonProfileViewModel : EntityViewModelBase<LegalPersonProfile>, IKazakhstanAdapted
    {
        [Dependency(DependencyType.NotRequiredDisableHide, "PositionInGenitive", "this.value=='NaturalPerson'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "PositionInNominative", "this.value=='NaturalPerson'")]
        public LegalPersonType LegalPersonType { get; set; }

        [SanitizedString]
        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string Name { get; set; }

        [RequiredLocalized]
        public LookupField LegalPerson { get; set; }

        [SanitizedString]
        [StringLengthLocalized(512, MinimumLength = 0)]
        public string DocumentsDeliveryAddress { get; set; }

        [SanitizedString]
        [StringLengthLocalized(256)]
        public string RecipientName { get; set; }

        [RequiredLocalized]
        [ExcludeZeroValue]
        [Dependency(DependencyType.Required, "DocumentsDeliveryAddress", "this.value == 'DeliveryByManager' || this.value == 'ByCourier'")]
        [Dependency(DependencyType.Required, "PostAddress", "this.value == 'PostOnly'")]
        [Dependency(DependencyType.Required, "EmailForAccountingDocuments", "this.value == 'ByEmail'")]
        public DocumentsDeliveryMethod DocumentsDeliveryMethod { get; set; }

        [SanitizedString]
        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string PersonResponsibleForDocuments { get; set; }

        [SanitizedString]
        [EmailLocalized]
        [StringLengthLocalized(64)]
        public string EmailForAccountingDocuments { get; set; }

        [SanitizedString]
        [EmailLocalized]
        [StringLengthLocalized(64)]
        public string Email { get; set; }

        [SanitizedString]
        [StringLengthLocalized(512)]
        public string PostAddress { get; set; }

        [SanitizedString]
        [StringLengthLocalized(512)]
        public string ActualAddress { get; set; }

        [RequiredLocalized]
        [Dependency(DependencyType.Required, "IBAN", "this.value == 'BankTransaction'")]
        [Dependency(DependencyType.Required, "SWIFT", "this.value == 'BankTransaction'")]
        [Dependency(DependencyType.Required, "BankName", "this.value == 'BankTransaction'")]
        public PaymentMethod PaymentMethod { get; set; }

        [StringLengthLocalized(20, MinimumLength = 20)]
        [RegularExpression(@"KZ[a-zA-Z0-9]{18}", ErrorMessageResourceName = @"SpecifiedIbanIsInvalid", ErrorMessageResourceType = typeof(Resources.Server.Properties.BLResources))]
        public string IBAN { get; set; }

        [StringLengthLocalized(11, MinimumLength = 8)]
        [RegularExpression(@"[a-zA-Z]{6}[^0-1][^O]([a-zA-Z0-9]{3})?", ErrorMessageResourceName = @"SpecifiedSwiftIsInvalid", ErrorMessageResourceType = typeof(Resources.Server.Properties.BLResources))]
        public string SWIFT { get; set; }

        [SanitizedString]
        [StringLengthLocalized(100)]
        public string BankName { get; set; }

        [SanitizedString]
        [StringLengthLocalized(512, MinimumLength = 0)]
        [DisplayNameLocalized("AdditionalPaymentElements")]
        public string PaymentEssentialElements { get; set; }

        [SanitizedString]
        [StringLengthLocalized(50)]
        [DisplayNameLocalized("ContactPhone")]
        public string Phone { get; set; }

        [RequiredLocalized]
        [Dependency(DependencyType.NotRequiredDisableHide, "CertificateNumber", "this.value!='Certificate'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "CertificateDate", "this.value!='Certificate'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "WarrantyNumber", "this.value!='Warranty'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "WarrantyEndDate", "this.value!='Warranty'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "WarrantyBeginDate", "this.value!='Warranty'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "DecreeNumber", "this.value!='Decree'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "DecreeDate", "this.value!='Decree'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "OtherAuthorityDocument", "this.value!='Other'")]
        public OperatesOnTheBasisType OperatesOnTheBasisInGenitive { get; set; }

        [SanitizedString]
        [StringLengthLocalized(256)]
        public string PositionInGenitive { get; set; }

        [SanitizedString]
        [StringLengthLocalized(256)]
        public string PositionInNominative { get; set; }

        [SanitizedString]
        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string ChiefNameInNominative { get; set; }

        [SanitizedString]
        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string ChiefNameInGenitive { get; set; }

        [SanitizedString]
        [StringLengthLocalized(50, MinimumLength = 0)]
        public string CertificateNumber { get; set; }

        public DateTime? CertificateDate { get; set; }

        [SanitizedString]
        [StringLengthLocalized(50, MinimumLength = 0)]
        public string DecreeNumber { get; set; }

        public DateTime? DecreeDate { get; set; }

        [SanitizedString]
        [StringLengthLocalized(50)]
        public string WarrantyNumber { get; set; }

        public DateTime? WarrantyBeginDate { get; set; }

        public DateTime? WarrantyEndDate { get; set; }

        public string OtherAuthorityDocument { get; set; }

        public override byte[] Timestamp { get; set; }

        public bool IsMainProfile { get; set; }

        // Нужно для отображения поля куратор во вкладке администрирования
        public override bool IsSecurityRoot
        {
            get { return true; }
        }

        public string[] DisabledDocuments { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (KazakhstanLegalPersonProfileDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            Name = modelDto.Name;
            Email = modelDto.Email;
            DocumentsDeliveryAddress = modelDto.DocumentsDeliveryAddress;
            DocumentsDeliveryMethod = modelDto.DocumentsDeliveryMethod;
            PaymentMethod = modelDto.PaymentMethod;

            LegalPerson = LookupField.FromReference(modelDto.LegalPersonRef);
            PostAddress = modelDto.PostAddress;
            Owner = LookupField.FromReference(modelDto.OwnerRef);
            EmailForAccountingDocuments = modelDto.EmailForAccountingDocuments;
            PersonResponsibleForDocuments = modelDto.PersonResponsibleForDocuments;
            Phone = modelDto.Phone;
            RecipientName = modelDto.RecipientName;
            IsMainProfile = modelDto.IsMainProfile;
            Timestamp = modelDto.Timestamp;
            LegalPersonType = modelDto.LegalPersonType;
            ChiefNameInGenitive = modelDto.ChiefNameInGenitive;
            ChiefNameInNominative = modelDto.ChiefNameInNominative;
            PositionInGenitive = modelDto.PositionInGenitive;
            PositionInNominative = modelDto.PositionInNominative;

            OperatesOnTheBasisInGenitive = modelDto.OperatesOnTheBasisInGenitive;

            OtherAuthorityDocument = modelDto.OtherAuthorityDocument;

            CertificateDate = modelDto.CertificateDate;
            CertificateNumber = modelDto.CertificateNumber;

            WarrantyBeginDate = modelDto.WarrantyBeginDate;
            WarrantyEndDate = modelDto.WarrantyEndDate;
            WarrantyNumber = modelDto.WarrantyNumber;

            DecreeNumber = modelDto.DecreeNumber;
            DecreeDate = modelDto.DecreeDate;

            BankName = modelDto.BankName;
            IBAN = modelDto.IBAN;
            SWIFT = modelDto.SWIFT;
            PaymentEssentialElements = modelDto.PaymentEssentialElements;

            ActualAddress = modelDto.ActualAddress;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new KazakhstanLegalPersonProfileDomainEntityDto
                       {
                           Id = Id,
                           Name = Name.EnsureСleanness(),
                           Email = Email.EnsureСleanness(),
                           DocumentsDeliveryAddress = DocumentsDeliveryAddress.EnsureСleanness(),
                           PaymentMethod = PaymentMethod,
                           DocumentsDeliveryMethod = DocumentsDeliveryMethod,
                           LegalPersonRef = LegalPerson.ToReference(),
                           PostAddress = PostAddress.EnsureСleanness(),
                           OwnerRef = Owner.ToReference(),
                           EmailForAccountingDocuments = EmailForAccountingDocuments.EnsureСleanness(),
                           PersonResponsibleForDocuments = PersonResponsibleForDocuments.EnsureСleanness(),
                           Phone = Phone.EnsureСleanness(),
                           RecipientName = RecipientName.EnsureСleanness(),
                           IsMainProfile = IsMainProfile,
                           Timestamp = Timestamp,
                           ChiefNameInGenitive = ChiefNameInGenitive.EnsureСleanness(),
                           ChiefNameInNominative = ChiefNameInNominative.EnsureСleanness(),
                           PositionInGenitive = PositionInGenitive.EnsureСleanness(),
                           PositionInNominative = PositionInNominative.EnsureСleanness(),

                           OperatesOnTheBasisInGenitive = OperatesOnTheBasisInGenitive,

                           OtherAuthorityDocument = OtherAuthorityDocument,

                           CertificateDate = CertificateDate,
                           CertificateNumber = CertificateNumber,

                           WarrantyBeginDate = WarrantyBeginDate,
                           WarrantyEndDate = WarrantyEndDate,
                           WarrantyNumber = WarrantyNumber,

                           DecreeNumber = DecreeNumber,
                           DecreeDate = DecreeDate,

                           BankName = BankName,
                           IBAN = IBAN,
                           SWIFT = SWIFT,
                           PaymentEssentialElements = PaymentEssentialElements,

                           ActualAddress = ActualAddress
                       };
        }
    }
}
