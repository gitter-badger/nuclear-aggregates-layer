using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Czech
{
    public sealed class CzechLegalPersonProfileViewModel : EntityViewModelBase<LegalPersonProfile>, ICzechAdapted
    {
        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string Name { get; set; }

        [Dependency(DependencyType.Required, "OperatesOnTheBasisInGenitive", "this.value!='NaturalPerson'")]
        [Dependency(DependencyType.Required, "Registered", "this.value=='LegalPerson'")]
        public LegalPersonType LegalPersonType { get; set; }

        [StringLengthLocalized(256)]
        public string PositionInGenitive { get; set; }

        [StringLengthLocalized(256)]
        public string PositionInNominative { get; set; }

        [StringLengthLocalized(150)]
        public string Registered { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string ChiefNameInNominative { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string ChiefNameInGenitive { get; set; }

        [Dependency(DependencyType.NotRequiredDisableHide, "WarrantyBeginDate", "this.value!='Warranty'")]
        public OperatesOnTheBasisType OperatesOnTheBasisInGenitive { get; set; }

        [Dependency(DependencyType.Required, "AccountNumber", "this.value=='BankTransaction'")]
        /* 
         * [Dependency(DependencyType.Required, "IBAN", "this.value=='BankTransaction'")]
         * [Dependency(DependencyType.Required, "SWIFT", "this.value=='BankTransaction'")]
         * [Dependency(DependencyType.Required, "BankAddress", "this.value=='BankTransaction'")]
         */
        [Dependency(DependencyType.Required, "BankCode", "this.value=='BankTransaction'")]
        [Dependency(DependencyType.Required, "BankName", "this.value=='BankTransaction'")]
        [RequiredLocalized]
        public PaymentMethod PaymentMethod { get; set; }

        [StringLengthLocalized(16)]
        public string AccountNumber { get; set; }

        [StringLengthLocalized(28, MinimumLength = 28)]
        public string IBAN { get; set; }

        [StringLengthLocalized(11, MinimumLength = 8)]
        public string SWIFT { get; set; }

        [StringLengthLocalized(4)]
        public string BankCode { get; set; }

        [StringLengthLocalized(100)]
        public string BankName { get; set; }

        [StringLengthLocalized(512, MinimumLength = 0)]
        public string BankAddress { get; set; }

        [StringLengthLocalized(512, MinimumLength = 0)]
        public string AdditionalPaymentElements { get; set; }

        [StringLengthLocalized(512, MinimumLength = 0)]
        public string DocumentsDeliveryAddress { get; set; }

        public DateTime? WarrantyBeginDate { get; set; }

        [StringLengthLocalized(512)]
        public string PostAddress { get; set; }

        [StringLengthLocalized(256)]
        public string RecipientName { get; set; }

        [RequiredLocalized]
        [Dependency(DependencyType.Required, "DocumentsDeliveryAddress", "this.value == 'PostOnly' || this.value == '' || this.value == 'ByCourier'")]
        [Dependency(DependencyType.Required, "EmailForAccountingDocuments", "this.value == 'ByEmail'")]
        public DocumentsDeliveryMethod DocumentsDeliveryMethod { get; set; }

        [EmailLocalized]
        [StringLengthLocalized(64)]
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
            Registered = modelDto.Registered;
            IBAN = modelDto.IBAN;
            BankCode = modelDto.BankCode;
            BankName = modelDto.BankName;
            BankAddress = modelDto.BankAddress;
            AdditionalPaymentElements = modelDto.AdditionalPaymentElements;
            SWIFT = modelDto.SWIFT;
            LegalPerson = LookupField.FromReference(modelDto.LegalPersonRef);
            PositionInNominative = modelDto.PositionInNominative;
            PositionInGenitive = modelDto.PositionInGenitive;
            OperatesOnTheBasisInGenitive = modelDto.OperatesOnTheBasisInGenitive;
            WarrantyBeginDate = modelDto.WarrantyBeginDate;
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
                    Registered = Registered,
                    DocumentsDeliveryAddress = DocumentsDeliveryAddress,
                    PaymentMethod = PaymentMethod,
                    AccountNumber = AccountNumber,
                    IBAN = IBAN,
                    SWIFT = SWIFT,
                    BankCode = BankCode,
                    BankName = BankName,
                    BankAddress = BankAddress,
                    AdditionalPaymentElements = AdditionalPaymentElements,
                    DocumentsDeliveryMethod = DocumentsDeliveryMethod,
                    LegalPersonRef = LegalPerson.ToReference(),
                    PositionInNominative = PositionInNominative,
                    PositionInGenitive = PositionInGenitive,
                    OperatesOnTheBasisInGenitive = OperatesOnTheBasisInGenitive,
                    WarrantyBeginDate = WarrantyBeginDate,
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