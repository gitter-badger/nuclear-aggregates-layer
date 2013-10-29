﻿using System;

using DoubleGis.Erm.Core.Enums;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.Web.Mvc.Utils;
using DoubleGis.Erm.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.UI.Web.Mvc.Models.Base;
using DoubleGis.Erm.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.UI.Web.Mvc.Models
{
    public sealed class CyprusLegalPersonProfileViewModel : EntityViewModelBase<LegalPersonProfile>, ICyprusAdapted
    {
        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string Name { get; set; }

        [Dependency(DependencyType.NotRequiredDisableHide, "PositionInNominative", "this.value=='NaturalPerson'")]
        [Dependency(DependencyType.Required, "PositionInNominative", "this.value!='NaturalPerson'")]
        [Dependency(DependencyType.Required, "OperatesOnTheBasisInGenitive", "this.value!='NaturalPerson'")]
        public LegalPersonType LegalPersonType { get; set; }

        [StringLengthLocalized(256)]
        public string PositionInGenitive { get; set; }

        [StringLengthLocalized(256)]
        public string PositionInNominative { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string ChiefNameInNominative { get; set; }

        [StringLengthLocalized(256)]
        public string ChiefNameInGenitive { get; set; }

        [Dependency(DependencyType.NotRequiredDisableHide, "CertificateNumber", "this.value!='Certificate'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "CertificateDate", "this.value!='Certificate'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "RegistrationCertificateNumber", "this.value!='RegistrationCertificate'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "RegistrationCertificateDate", "this.value!='RegistrationCertificate'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "WarrantyNumber", "this.value!='Warranty'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "WarrantyEndDate", "this.value!='Warranty'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "WarrantyBeginDate", "this.value!='Warranty'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "BargainNumber", "this.value!='Bargain'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "BargainEndDate", "this.value!='Bargain'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "BargainBeginDate", "this.value!='Bargain'")]
        public OperatesOnTheBasisType OperatesOnTheBasisInGenitive { get; set; }

        [Dependency(DependencyType.Required, "AccountNumber", "this.value=='BankTransaction' || this.value=='BankChequePayment'")]
        [Dependency(DependencyType.Required, "IBAN", "this.value=='BankTransaction'")]
        [Dependency(DependencyType.Required, "SWIFT", "this.value=='BankTransaction'")]
        [Dependency(DependencyType.Required, "BankName", "this.value=='BankTransaction' || this.value=='BankChequePayment'")]
        [RequiredLocalized]
        public PaymentMethod PaymentMethod { get; set; }

        [StringLengthLocalized(16, MinimumLength = 0)]
        public string AccountNumber { get; set; }

        [StringLengthLocalized(28, MinimumLength = 28)]
        public string IBAN { get; set; }

        [StringLengthLocalized(11, MinimumLength = 8)]
        public string SWIFT { get; set; }

        [StringLengthLocalized(100, MinimumLength = 0)]
        public string BankName { get; set; }

        [StringLengthLocalized(512, MinimumLength = 0)]
        public string AdditionalPaymentElements { get; set; }

        [StringLengthLocalized(512, MinimumLength = 0)]
        public string DocumentsDeliveryAddress { get; set; }

        [StringLengthLocalized(50, MinimumLength = 0)]
        public string CertificateNumber { get; set; }

        public DateTime? CertificateDate { get; set; }

        [StringLengthLocalized(9, MinimumLength = 0)]
        public string RegistrationCertificateNumber { get; set; }

        public DateTime? RegistrationCertificateDate { get; set; }

        [StringLengthLocalized(50)]
        public string WarrantyNumber { get; set; }

        public DateTime? WarrantyBeginDate { get; set; }

        public DateTime? WarrantyEndDate { get; set; }

        [StringLengthLocalized(50)]
        public string BargainNumber { get; set; }

        public DateTime? BargainBeginDate { get; set; }

        public DateTime? BargainEndDate { get; set; }

        [StringLengthLocalized(512)]
        [RequiredLocalized]
        public string PostAddress { get; set; }

        [StringLengthLocalized(256)]
        public string RecipientName { get; set; }

        [Dependency(DependencyType.Required, "DocumentsDeliveryAddress", "this.value == 'PostOnly' || this.value == '' || this.value == 'ByCourier'")]
        public DocumentsDeliveryMethod DocumentsDeliveryMethod { get; set; }

        [EmailLocalized]
        [StringLengthLocalized(64)]
        [RequiredLocalized]
        public string EmailForAccountingDocuments { get; set; }

        [EmailLocalized]
        [StringLengthLocalized(64)]
        public string AdditionalEmail { get; set; }

        [StringLengthLocalized(256)]
        [RequiredLocalized]
        public string PersonResponsibleForDocuments { get; set; }

        [DisplayNameLocalized("ContactPhone")]
        [StringLengthLocalized(50)]
        public string Phone { get; set; }

        [StringLengthLocalized(512)]
        public string PaymentEssentialElements { get; set; }

        public override byte[] Timestamp { get; set; }

        [RequiredLocalized]
        public override LookupField Owner { get; set; }

        [RequiredLocalized]
        public LookupField LegalPerson { get; set; }

        public bool IsMainProfile { get; set; }

        // Нужно для отображения поля куратор во вкладке администрирования
        public override bool IsSecurityRoot
        {
            get
            {
                return true;
            }
        }

        public int[] DisabledDocuments { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (LegalPersonProfileDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            Name = modelDto.Name;
            AdditionalEmail = modelDto.AdditionalEmail;
            ChiefNameInGenitive = modelDto.ChiefNameInGenitive;
            ChiefNameInNominative = modelDto.ChiefNameInNominative;
            DocumentsDeliveryAddress = modelDto.DocumentsDeliveryAddress;
            DocumentsDeliveryMethod = modelDto.DocumentsDeliveryMethod;
            PaymentMethod = modelDto.PaymentMethod;
            AccountNumber = modelDto.AccountNumber;
            IBAN = modelDto.IBAN;
            BankName = modelDto.BankName;
            AdditionalPaymentElements = modelDto.AdditionalPaymentElements;
            SWIFT = modelDto.SWIFT;
            LegalPerson = LookupField.FromReference(modelDto.LegalPersonRef);
            PositionInNominative = modelDto.PositionInNominative;
            PositionInGenitive = modelDto.PositionInGenitive;
            OperatesOnTheBasisInGenitive = modelDto.OperatesOnTheBasisInGenitive;
            CertificateDate = modelDto.CertificateDate;
            CertificateNumber = modelDto.CertificateNumber;
            RegistrationCertificateNumber = modelDto.RegistrationCertificateNumber;
            RegistrationCertificateDate = modelDto.RegistrationCertificateDate;
            BargainBeginDate = modelDto.BargainBeginDate;
            BargainEndDate = modelDto.BargainEndDate;
            BargainNumber = modelDto.BargainNumber;
            WarrantyNumber = modelDto.WarrantyNumber;
            WarrantyBeginDate = modelDto.WarrantyBeginDate;
            WarrantyEndDate = modelDto.WarrantyEndDate;
            PostAddress = modelDto.PostAddress;
            Owner = LookupField.FromReference(modelDto.OwnerRef);
            EmailForAccountingDocuments = modelDto.EmailForAccountingDocuments;
            LegalPersonType = modelDto.LegalPersonType;
            PaymentEssentialElements = modelDto.PaymentEssentialElements;
            PersonResponsibleForDocuments = modelDto.PersonResponsibleForDocuments;
            Phone = modelDto.Phone;
            RecipientName = modelDto.RecipientName;
            IsMainProfile = modelDto.IsMainProfile;
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new LegalPersonProfileDomainEntityDto
                {
                    Id = Id,
                    Name = Name,
                    AdditionalEmail = AdditionalEmail,
                    ChiefNameInGenitive = ChiefNameInGenitive,
                    ChiefNameInNominative = ChiefNameInNominative,
                    DocumentsDeliveryAddress = DocumentsDeliveryAddress,
                    PaymentMethod = PaymentMethod,
                    AccountNumber = AccountNumber,
                    IBAN = IBAN,
                    SWIFT = SWIFT,
                    BankName = BankName,
                    AdditionalPaymentElements = AdditionalPaymentElements,
                    DocumentsDeliveryMethod = DocumentsDeliveryMethod,
                    LegalPersonRef = LegalPerson.ToReference(),
                    PositionInNominative = PositionInNominative,
                    PositionInGenitive = PositionInGenitive,
                    OperatesOnTheBasisInGenitive = OperatesOnTheBasisInGenitive,
                    CertificateDate = CertificateDate,
                    CertificateNumber = CertificateNumber,
                    RegistrationCertificateDate = RegistrationCertificateDate,
                    RegistrationCertificateNumber = RegistrationCertificateNumber,
                    BargainBeginDate = BargainBeginDate,
                    BargainEndDate = BargainEndDate,
                    BargainNumber = BargainNumber,
                    WarrantyNumber = WarrantyNumber,
                    WarrantyBeginDate = WarrantyBeginDate,
                    WarrantyEndDate = WarrantyEndDate,
                    PostAddress = PostAddress,
                    OwnerRef = Owner.ToReference(),
                    EmailForAccountingDocuments = EmailForAccountingDocuments,
                    LegalPersonType = LegalPersonType,
                    PaymentEssentialElements = PaymentEssentialElements,
                    PersonResponsibleForDocuments = PersonResponsibleForDocuments,
                    Phone = Phone,
                    RecipientName = RecipientName,
                    IsMainProfile = IsMainProfile,
                    Timestamp = Timestamp
                };
        }
    }
}