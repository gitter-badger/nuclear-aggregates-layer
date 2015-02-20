using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Ukraine;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Aspects.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Ukraine
{
    public sealed class UkraineLegalPersonProfileViewModel : EntityViewModelBase<LegalPersonProfile>, IMainLegalPersonProfileAspect, INameAspect, IUkraineAdapted
    {
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

        [RequiredLocalized]
        [Dependency(DependencyType.Required, "BankName", "this.value == 'BankTransaction'")]
        [Dependency(DependencyType.Required, "Mfo", "this.value == 'BankTransaction'")]
        [Dependency(DependencyType.Required, "AccountNumber", "this.value == 'BankTransaction'")]
        public PaymentMethod PaymentMethod { get; set; }

        [SanitizedString]
        [StringLengthLocalized(14)]
        public string AccountNumber { get; set; }

        [SanitizedString]
        [StringLengthLocalized(6, MinimumLength = 6)]
        [OnlyDigitsLocalized]
        public string Mfo { get; set; }

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
        public OperatesOnTheBasisType OperatesOnTheBasisInGenitive { get; set; }

        [SanitizedString]
        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string PositionInGenitive { get; set; }

        [SanitizedString]
        [RequiredLocalized]
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
        [StringLengthLocalized(50)]
        public string WarrantyNumber { get; set; }

        public DateTime? WarrantyBeginDate { get; set; }

        public DateTime? WarrantyEndDate { get; set; }

        public override byte[] Timestamp { get; set; }

        public bool IsMainProfile { get; set; }

        // Нужно для отображения поля куратор во вкладке администрирования
        public override bool IsSecurityRoot
        {
            get
            {
                return true;
            }
        }

        public string[] DisabledDocuments { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (UkraineLegalPersonProfileDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            Name = modelDto.Name;
            Email = modelDto.AdditionalEmail;
            DocumentsDeliveryAddress = modelDto.DocumentsDeliveryAddress;
            DocumentsDeliveryMethod = modelDto.DocumentsDeliveryMethod;
            PaymentMethod = modelDto.PaymentMethod;
            AccountNumber = modelDto.AccountNumber;
            BankName = modelDto.BankName;
            PaymentEssentialElements = modelDto.PaymentEssentialElements;
            LegalPerson = LookupField.FromReference(modelDto.LegalPersonRef);
            PostAddress = modelDto.PostAddress;
            Owner = LookupField.FromReference(modelDto.OwnerRef);
            EmailForAccountingDocuments = modelDto.EmailForAccountingDocuments;
            PersonResponsibleForDocuments = modelDto.PersonResponsibleForDocuments;
            Phone = modelDto.Phone;
            RecipientName = modelDto.RecipientName;
            IsMainProfile = modelDto.IsMainProfile;
            Timestamp = modelDto.Timestamp;
            OperatesOnTheBasisInGenitive = modelDto.OperatesOnTheBasisInGenitive;
            LegalPersonType = modelDto.LegalPersonType;
            ChiefNameInGenitive = modelDto.ChiefNameInGenitive;
            ChiefNameInNominative = modelDto.ChiefNameInNominative;
            PositionInGenitive = modelDto.PositionInGenitive;
            PositionInNominative = modelDto.PositionInNominative;
            CertificateDate = modelDto.CertificateDate;
            CertificateNumber = modelDto.CertificateNumber;
            WarrantyBeginDate = modelDto.WarrantyBeginDate;
            WarrantyEndDate = modelDto.WarrantyEndDate;
            WarrantyNumber = modelDto.WarrantyNumber;
            Mfo = modelDto.Mfo;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new UkraineLegalPersonProfileDomainEntityDto
            {
                Id = Id,
                Name = Name.EnsureСleanness(),
                AdditionalEmail = Email.EnsureСleanness(),
                DocumentsDeliveryAddress = DocumentsDeliveryAddress.EnsureСleanness(),
                PaymentMethod = PaymentMethod,
                AccountNumber = AccountNumber.EnsureСleanness(),
                PaymentEssentialElements = PaymentEssentialElements.EnsureСleanness(),
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
                OperatesOnTheBasisInGenitive = OperatesOnTheBasisInGenitive,
                Mfo = Mfo.EnsureСleanness(),
                BankName = BankName.EnsureСleanness(),
                ChiefNameInGenitive = ChiefNameInGenitive.EnsureСleanness(),
                ChiefNameInNominative = ChiefNameInNominative.EnsureСleanness(),
                PositionInGenitive = PositionInGenitive.EnsureСleanness(),
                PositionInNominative = PositionInNominative.EnsureСleanness(),
                CertificateDate = CertificateDate,
                CertificateNumber = CertificateNumber.EnsureСleanness(),
                WarrantyBeginDate = WarrantyBeginDate,
                WarrantyEndDate = WarrantyEndDate,
                WarrantyNumber = WarrantyNumber.EnsureСleanness()
            };
        }
    }
}