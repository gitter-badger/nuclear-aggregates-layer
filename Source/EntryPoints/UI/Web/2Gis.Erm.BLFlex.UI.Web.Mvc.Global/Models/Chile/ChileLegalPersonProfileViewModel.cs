using System;

using DoubleGis.Erm.BL.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Chile
{
    public sealed class ChileLegalPersonProfileViewModel : EntityViewModelBase<LegalPersonProfile>, IChileAdapted
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
        [Dependency(DependencyType.Required, "Bank", BankFieldsAreRequired)]
        [Dependency(DependencyType.Required, "AccountNumber", BankFieldsAreRequired)]
        [Dependency(DependencyType.Required, "BankAccountType", BankFieldsAreRequired)]
        public PaymentMethod PaymentMethod { get; set; }

        public AccountType BankAccountType { get; set; }

        [StringLengthLocalized(24)]
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

        // ����� ��� ����������� ���� ������� �� ������� �����������������
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
            var modelDto = (ChileLegalPersonProfileDomainEntityDto)domainEntityDto;

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
            PersonResponsibleForDocuments = modelDto.PersonResponsibleForDocuments;
            Phone = modelDto.Phone;
            RecipientName = modelDto.RecipientName;
            IsMainProfile = modelDto.IsMainProfile;
            Timestamp = modelDto.Timestamp;
            Bank = modelDto.BankRef.ToLookupField();
            BankAccountType = modelDto.AccountType;
            RepresentativeName = modelDto.RepresentativeName;
            RepresentativeRut = modelDto.RepresentativeRut;
            RepresentativePosition = modelDto.RepresentativePosition;
            OperatesOnTheBasisInGenitive = modelDto.OperatesOnTheBasisInGenitive;

            RepresentativeDocumentIssuedOn = modelDto.RepresentativeDocumentIssuedOn;
            RepresentativeDocumentIssuedBy = modelDto.RepresentativeDocumentIssuedBy;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new ChileLegalPersonProfileDomainEntityDto
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
                    PersonResponsibleForDocuments = PersonResponsibleForDocuments,
                    Phone = Phone,
                    RecipientName = RecipientName,
                    IsMainProfile = IsMainProfile,
                    Timestamp = Timestamp,
                    BankRef = Bank.ToReference(),
                    AccountType = BankAccountType,
                    RepresentativeName = RepresentativeName,
                    RepresentativeRut = RepresentativeRut,
                    RepresentativePosition = RepresentativePosition,
                    OperatesOnTheBasisInGenitive = OperatesOnTheBasisInGenitive,

                    RepresentativeDocumentIssuedBy = RepresentativeDocumentIssuedBy,
                    RepresentativeDocumentIssuedOn = RepresentativeDocumentIssuedOn,
                };
        }
    }
}