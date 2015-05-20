using System;

using DoubleGis.Erm.BL.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Chile;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Aspects.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Chile
{
    public sealed class ChileLegalPersonProfileViewModel : EntityViewModelBase<LegalPersonProfile>, IMainLegalPersonProfileAspect, INameAspect, IChileAdapted
    {
        private const string BankFieldsAreRequired =
            "this.value=='CreditCard' || this.value=='DebitCard' || this.value=='BankChequePayment' || this.value=='BankTransaction'";

        private const string RepresentativeDocumentFieldsAreHidden =
            "this.value!='Charter' && this.value!='Warranty'";

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
        [Dependency(DependencyType.Required, "DocumentsDeliveryAddress", "this.value == 'DeliveryByManager' || this.value == 'ByCourier'")]
        [Dependency(DependencyType.Required, "EmailForAccountingDocuments", "this.value == 'ByEmail'")]
        [Dependency(DependencyType.Required, "PostAddress", "this.value == 'PostOnly'")]
        public DocumentsDeliveryMethod DocumentsDeliveryMethod { get; set; }
        
        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string PersonResponsibleForDocuments { get; set; }

        [EmailLocalized]
        [StringLengthLocalized(64)]
        public string EmailForAccountingDocuments { get; set; }

        [EmailLocalized]
        [StringLengthLocalized(64)]
        public string Email { get; set; }

        [StringLengthLocalized(512)]
        public string PostAddress { get; set; }

        [RequiredLocalized]
        [Dependency(DependencyType.Required, "Bank", BankFieldsAreRequired)]
        [Dependency(DependencyType.Required, "AccountNumber", BankFieldsAreRequired)]
        [Dependency(DependencyType.Required, "BankAccountType", BankFieldsAreRequired)]
        public PaymentMethod PaymentMethod { get; set; }

        public AccountType BankAccountType { get; set; }

        [StringLengthLocalized(24)]
        public string AccountNumber { get; set; }

        public LookupField Bank { get; set; }

        [StringLengthLocalized(512, MinimumLength = 0)]
        [DisplayNameLocalized("AdditionalPaymentElements")]
        public string PaymentEssentialElements { get; set; }

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
        [Dependency(DependencyType.NotRequiredDisableHide, "RepresentativeDocumentIssuedOn", RepresentativeDocumentFieldsAreHidden)]
        [Dependency(DependencyType.NotRequiredDisableHide, "RepresentativeDocumentIssuedBy", RepresentativeDocumentFieldsAreHidden)]
        public OperatesOnTheBasisType OperatesOnTheBasisInGenitive { get; set; }

        [RequiredLocalized]
        public DateTime? RepresentativeDocumentIssuedOn { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string RepresentativeDocumentIssuedBy { get; set; }

        public override byte[] Timestamp { get; set; }

        public bool IsMainProfile { get; set; }

        // ÕÛÊÌÓ ‰Îˇ ÓÚÓ·‡ÊÂÌËˇ ÔÓÎˇ ÍÛ‡ÚÓ ‚Ó ‚ÍÎ‡‰ÍÂ ‡‰ÏËÌËÒÚËÓ‚‡ÌËˇ
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
            var modelDto = (ChileLegalPersonProfileDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            Name = modelDto.Name;
            Email = modelDto.Email;
            DocumentsDeliveryAddress = modelDto.DocumentsDeliveryAddress;
            DocumentsDeliveryMethod = modelDto.DocumentsDeliveryMethod;
            PaymentMethod = modelDto.PaymentMethod;
            AccountNumber = modelDto.AccountNumber;
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
            Bank = modelDto.BankRef.ToLookupField();
            BankAccountType = modelDto.AccountType;
            RepresentativeName = modelDto.ChiefNameInNominative;
            RepresentativeRut = modelDto.RepresentativeRut;
            RepresentativePosition = modelDto.PositionInNominative;
            OperatesOnTheBasisInGenitive = modelDto.OperatesOnTheBasisInGenitive;

            RepresentativeDocumentIssuedOn = modelDto.RepresentativeDocumentIssuedOn;
            RepresentativeDocumentIssuedBy = modelDto.RepresentativeDocumentIssuedBy;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new ChileLegalPersonProfileDomainEntityDto
                {
                    Id = Id,
                    Name = Name.Ensure—leanness(),
                    Email = Email.Ensure—leanness(),
                    DocumentsDeliveryAddress = DocumentsDeliveryAddress.Ensure—leanness(),
                    PaymentMethod = PaymentMethod,
                    AccountNumber = AccountNumber.Ensure—leanness(),
                    PaymentEssentialElements = PaymentEssentialElements.Ensure—leanness(),
                    DocumentsDeliveryMethod = DocumentsDeliveryMethod,
                    LegalPersonRef = LegalPerson.ToReference(),
                    PostAddress = PostAddress.Ensure—leanness(),
                    OwnerRef = Owner.ToReference(),
                    EmailForAccountingDocuments = EmailForAccountingDocuments.Ensure—leanness(),
                    PersonResponsibleForDocuments = PersonResponsibleForDocuments.Ensure—leanness(),
                    Phone = Phone.Ensure—leanness(),
                    RecipientName = RecipientName.Ensure—leanness(),
                    IsMainProfile = IsMainProfile,
                    Timestamp = Timestamp,
                    BankRef = Bank.ToReference(),
                    AccountType = BankAccountType,
                    ChiefNameInNominative = RepresentativeName.Ensure—leanness(),
                    RepresentativeRut = RepresentativeRut.Ensure—leanness(),
                    PositionInNominative = RepresentativePosition.Ensure—leanness(),
                    OperatesOnTheBasisInGenitive = OperatesOnTheBasisInGenitive,

                    RepresentativeDocumentIssuedBy = RepresentativeDocumentIssuedBy,
                    RepresentativeDocumentIssuedOn = RepresentativeDocumentIssuedOn,
                };
        }
    }
}