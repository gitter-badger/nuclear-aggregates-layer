using System.ComponentModel.DataAnnotations;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Emirates;
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

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Emirates
{
    public sealed class EmiratesLegalPersonProfileViewModel : EntityViewModelBase<LegalPersonProfile>, IMainLegalPersonProfileAspect, INameAspect, IEmiratesAdapted
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

        [StringLengthLocalized(256)]
        [RequiredLocalized]
        public string PersonResponsibleForDocuments { get; set; }

        [RequiredLocalized]
        [Dependency(DependencyType.Required, "PostAddress", "this.value == 'PostOnly'")]
        [Dependency(DependencyType.Required, "EmailForAccountingDocuments", "this.value == 'ByEmail'")]
        [Dependency(DependencyType.Required, "DocumentsDeliveryAddress", "this.value == 'ByCourier' || this.value == 'DeliveryByManager'")]
        public DocumentsDeliveryMethod DocumentsDeliveryMethod { get; set; }

        [EmailLocalized]
        [StringLengthLocalized(64)]
        public string EmailForAccountingDocuments { get; set; }

        [EmailLocalized]
        [StringLengthLocalized(64)]
        public string Email { get; set; }

        [StringLengthLocalized(512)]
        public string PostAddress { get; set; }

        [StringLengthLocalized(50)]
        public string PhoneNumber { get; set; }

        [StringLengthLocalized(50)]
        public string Fax { get; set; }

        [Dependency(DependencyType.Required, "IBAN", "this.value=='BankTransaction'||this.value=='BankChequePayment'")]
        [Dependency(DependencyType.Required, "SWIFT", "this.value=='BankTransaction'||this.value=='BankChequePayment'")]
        [RequiredLocalized]
        public PaymentMethod PaymentMethod { get; set; }

        [StringLengthLocalized(23, MinimumLength = 23)]
        [RegularExpression(@"AE\d{21}", ErrorMessageResourceName = @"SpecifiedIbanIsInvalid", ErrorMessageResourceType = typeof(Resources.Server.Properties.BLResources))]
        public string IBAN { get; set; }

        [StringLengthLocalized(11, MinimumLength = 8)]
        [RegularExpression(@"[a-zA-Z]{6}[^0-1][^O]([a-zA-Z0-9]{3})?", ErrorMessageResourceName = @"SpecifiedSwiftIsInvalid", ErrorMessageResourceType = typeof(Resources.Server.Properties.BLResources))]
        public string SWIFT { get; set; }

        [StringLengthLocalized(100, MinimumLength = 0)]
        public string BankName { get; set; }

        [StringLengthLocalized(512, MinimumLength = 0)]
        [DisplayNameLocalized("AdditionalPaymentElements")]
        public string PaymentEssentialElements { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string ChiefNameInNominative { get; set; }

        [StringLengthLocalized(256)]
        [RequiredLocalized]
        public string PositionInNominative { get; set; }

        public override byte[] Timestamp { get; set; }

        [RequiredLocalized]
        public override LookupField Owner { get; set; }

        public bool IsMainProfile { get; set; }

        // Нужно для отображения поля куратор во вкладке администрирования
        public override bool IsSecurityRoot
        {
            get { return true; }
        }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (EmiratesLegalPersonProfileDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            Name = modelDto.Name;
            Email = modelDto.Email;
            ChiefNameInNominative = modelDto.ChiefNameInNominative;
            DocumentsDeliveryAddress = modelDto.DocumentsDeliveryAddress;
            DocumentsDeliveryMethod = modelDto.DocumentsDeliveryMethod;
            PaymentMethod = modelDto.PaymentMethod;
            IBAN = modelDto.IBAN;
            BankName = modelDto.BankName;
            PaymentEssentialElements = modelDto.PaymentEssentialElements;
            SWIFT = modelDto.SWIFT;
            LegalPerson = LookupField.FromReference(modelDto.LegalPersonRef);
            PositionInNominative = modelDto.PositionInNominative;
            PostAddress = modelDto.PostAddress;
            Owner = LookupField.FromReference(modelDto.OwnerRef);
            EmailForAccountingDocuments = modelDto.EmailForAccountingDocuments;
            PersonResponsibleForDocuments = modelDto.PersonResponsibleForDocuments;
            PhoneNumber = modelDto.Phone;
            Fax = modelDto.Fax;
            RecipientName = modelDto.RecipientName;
            IsMainProfile = modelDto.IsMainProfile;
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new EmiratesLegalPersonProfileDomainEntityDto
                {
                    Id = Id,
                    Name = Name.EnsureСleanness(),
                    ChiefNameInNominative = ChiefNameInNominative.EnsureСleanness(),
                    DocumentsDeliveryAddress = DocumentsDeliveryAddress.EnsureСleanness(),
                    PaymentMethod = PaymentMethod,
                    IBAN = IBAN.EnsureСleanness(),
                    SWIFT = SWIFT.EnsureСleanness(),
                    BankName = BankName.EnsureСleanness(),
                    PaymentEssentialElements = PaymentEssentialElements.EnsureСleanness(),
                    DocumentsDeliveryMethod = DocumentsDeliveryMethod,
                    LegalPersonRef = LegalPerson.ToReference(),
                    PositionInNominative = PositionInNominative.EnsureСleanness(),
                    PostAddress = PostAddress.EnsureСleanness(),
                    OwnerRef = Owner.ToReference(),
                    Email = Email.EnsureСleanness(),
                    EmailForAccountingDocuments = EmailForAccountingDocuments.EnsureСleanness(),
                    PersonResponsibleForDocuments = PersonResponsibleForDocuments.EnsureСleanness(),
                    Phone = PhoneNumber.EnsureСleanness(),
                    Fax = Fax.EnsureСleanness(),
                    RecipientName = RecipientName.EnsureСleanness(),
                    IsMainProfile = IsMainProfile,
                    Timestamp = Timestamp
                };
        }
    }
}