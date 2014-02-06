using System;

using DoubleGis.Erm.BL.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Chile
{
    public sealed class ChileLegalPersonProfileViewModel : EntityViewModelBase<LegalPersonProfile>, IChileAdapted
    {
        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string Name { get; set; }

        [RequiredLocalized]
        public LookupField LegalPerson { get; set; }

        [StringLengthLocalized(512, MinimumLength = 0)]
        public string DocumentsDeliveryAddress { get; set; }

        [StringLengthLocalized(256)]
        public string RecipientName { get; set; }

        [RequiredLocalized]
        [Dependency(DependencyType.Required, "DocumentsDeliveryAddress", "this.value == 'PostOnly' || this.value == 'DeliveryByManager' || this.value == 'ByCourier'")]
        [Dependency(DependencyType.Required, "EmailForAccountingDocuments", "this.value == 'ByEmail'")]
        public DocumentsDeliveryMethod DocumentsDeliveryMethod { get; set; }
        
        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string PersonResponsibleForDocuments { get; set; }

        [EmailLocalized]
        [StringLengthLocalized(64)]
        public string EmailForAccountingDocuments { get; set; }

        [EmailLocalized]
        [StringLengthLocalized(64)]
        public string AdditionalEmail { get; set; }

        [StringLengthLocalized(512)]
        public string PostAddress { get; set; }

        [RequiredLocalized]
        [Dependency(DependencyType.Required, "Bank", "this.value=='BankTransaction'")]
        [Dependency(DependencyType.Required, "AccountNumber", "this.value=='CreditCardPayment' || this.value=='DebitCard' || this.value=='BankChequePayment' || this.value=='BankTransaction'")]
        [Dependency(DependencyType.Required, "BankAccountType", "this.value=='CreditCardPayment' || this.value=='DebitCard' || this.value=='BankChequePayment' || this.value=='BankTransaction'")]
        public PaymentMethod PaymentMethod { get; set; }

        public BankAccountType BankAccountType { get; set; }

        [StringLengthLocalized(16)]
        public string AccountNumber { get; set; }

        public LookupField Bank { get; set; }

        [StringLengthLocalized(512, MinimumLength = 0)]
        public string AdditionalPaymentElements { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string RepresentativeName { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string RepresentativePosition { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(256)]
        [RutValidation]
        public string RepresentativeRut { get; set; }

        [StringLengthLocalized(50)]
        [DisplayNameLocalized("ContactPhone")]
        public string Phone { get; set; }

        [RequiredLocalized]
        [Dependency(DependencyType.NotRequiredDisableHide, "WarrantyBeginDate", "this.value!='Warranty'")]
        public OperatesOnTheBasisType OperatesOnTheBasis { get; set; }

        [RequiredLocalized]
        public DateTime DocumentBeginDate { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string DocumentProvider { get; set; }

        public LegalPersonType LegalPersonType { get; set; }

        public override byte[] Timestamp { get; set; }

        public override LookupField Owner { get; set; }

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
            DocumentsDeliveryAddress = modelDto.DocumentsDeliveryAddress;
            DocumentsDeliveryMethod = modelDto.DocumentsDeliveryMethod;
            PaymentMethod = modelDto.PaymentMethod;
            AccountNumber = modelDto.AccountNumber;
            AdditionalPaymentElements = modelDto.AdditionalPaymentElements;
            LegalPerson = LookupField.FromReference(modelDto.LegalPersonRef);
            PostAddress = modelDto.PostAddress;
            Owner = LookupField.FromReference(modelDto.OwnerRef);
            EmailForAccountingDocuments = modelDto.EmailForAccountingDocuments;
            LegalPersonType = modelDto.LegalPersonType;
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
                    DocumentsDeliveryAddress = DocumentsDeliveryAddress,
                    PaymentMethod = PaymentMethod,
                    AccountNumber = AccountNumber,
                    AdditionalPaymentElements = AdditionalPaymentElements,
                    DocumentsDeliveryMethod = DocumentsDeliveryMethod,
                    LegalPersonRef = LegalPerson.ToReference(),
                    PostAddress = PostAddress,
                    OwnerRef = Owner.ToReference(),
                    EmailForAccountingDocuments = EmailForAccountingDocuments,
                    LegalPersonType = LegalPersonType,
                    PersonResponsibleForDocuments = PersonResponsibleForDocuments,
                    Phone = Phone,
                    RecipientName = RecipientName,
                    IsMainProfile = IsMainProfile,
                    Timestamp = Timestamp
                };
        }
    }
}