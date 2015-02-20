using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Aspects.Entities;
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
    public sealed class CzechLegalPersonProfileViewModel : EntityViewModelBase<LegalPersonProfile>, IMainLegalPersonProfileAspect, INameAspect, ICzechAdapted
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
        [Dependency(DependencyType.Required, "BankCode", "this.value=='BankTransaction'")]
        [Dependency(DependencyType.Required, "BankName", "this.value=='BankTransaction'")]
        [RequiredLocalized]
        public PaymentMethod PaymentMethod { get; set; }

        [StringLengthLocalized(10)]
        public string AccountNumber { get; set; }

        [StringLengthLocalized(4)]
        public string BankCode { get; set; }

        [StringLengthLocalized(100)]
        public string BankName { get; set; }

        [StringLengthLocalized(512, MinimumLength = 0)]
        public string BankAddress { get; set; }

        [StringLengthLocalized(512, MinimumLength = 0)]
        [DisplayNameLocalized("AdditionalPaymentElements")]
        public string PaymentEssentialElements { get; set; }

        [StringLengthLocalized(512, MinimumLength = 0)]
        public string DocumentsDeliveryAddress { get; set; }

        public DateTime? WarrantyBeginDate { get; set; }

        [StringLengthLocalized(512)]
        public string PostAddress { get; set; }

        [StringLengthLocalized(256)]
        public string RecipientName { get; set; }

        [RequiredLocalized]
        [Dependency(DependencyType.Required, "DocumentsDeliveryAddress", "this.value == 'ByCourier'")]
        [Dependency(DependencyType.Required, "EmailForAccountingDocuments", "this.value == 'ByEmail'")]
        public DocumentsDeliveryMethod DocumentsDeliveryMethod { get; set; }

        [EmailLocalized]
        [StringLengthLocalized(64)]
        public string EmailForAccountingDocuments { get; set; }

        [EmailLocalized]
        [StringLengthLocalized(64)]
        public string Email { get; set; }

        [StringLengthLocalized(256)]
        [RequiredLocalized]
        public string PersonResponsibleForDocuments { get; set; }

        [DisplayNameLocalized("ContactPhone")]
        [StringLengthLocalized(50)]
        public string Phone { get; set; }

        public override byte[] Timestamp { get; set; }

        [RequiredLocalized]
        public override LookupField Owner { get; set; }

        [RequiredLocalized]
        public LookupField LegalPerson { get; set; }

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
            var modelDto = (LegalPersonProfileDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            Name = modelDto.Name;
            Email = modelDto.AdditionalEmail;
            ChiefNameInGenitive = modelDto.ChiefNameInGenitive;
            ChiefNameInNominative = modelDto.ChiefNameInNominative;
            DocumentsDeliveryAddress = modelDto.DocumentsDeliveryAddress;
            DocumentsDeliveryMethod = modelDto.DocumentsDeliveryMethod;
            PaymentMethod = modelDto.PaymentMethod;
            AccountNumber = modelDto.AccountNumber;
            Registered = modelDto.Registered;
            BankCode = modelDto.BankCode;
            BankName = modelDto.BankName;
            BankAddress = modelDto.BankAddress;
            PaymentEssentialElements = modelDto.PaymentEssentialElements;
            LegalPerson = LookupField.FromReference(modelDto.LegalPersonRef);
            PositionInNominative = modelDto.PositionInNominative;
            PositionInGenitive = modelDto.PositionInGenitive;
            OperatesOnTheBasisInGenitive = modelDto.OperatesOnTheBasisInGenitive;
            WarrantyBeginDate = modelDto.WarrantyBeginDate;
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
                    Name = Name.Ensure—leanness(),
                    AdditionalEmail = Email.Ensure—leanness(),
                    ChiefNameInGenitive = ChiefNameInGenitive.Ensure—leanness(),
                    ChiefNameInNominative = ChiefNameInNominative.Ensure—leanness(),
                    Registered = Registered.Ensure—leanness(),
                    DocumentsDeliveryAddress = DocumentsDeliveryAddress.Ensure—leanness(),
                    PaymentMethod = PaymentMethod,
                    AccountNumber = AccountNumber.Ensure—leanness(),
                    BankCode = BankCode.Ensure—leanness(),
                    BankName = BankName.Ensure—leanness(),
                    BankAddress = BankAddress.Ensure—leanness(),
                    PaymentEssentialElements = PaymentEssentialElements.Ensure—leanness(),
                    DocumentsDeliveryMethod = DocumentsDeliveryMethod,
                    LegalPersonRef = LegalPerson.ToReference(),
                    PositionInNominative = PositionInNominative.Ensure—leanness(),
                    PositionInGenitive = PositionInGenitive.Ensure—leanness(),
                    OperatesOnTheBasisInGenitive = OperatesOnTheBasisInGenitive,
                    WarrantyBeginDate = WarrantyBeginDate,
                    PostAddress = PostAddress.Ensure—leanness(),
                    OwnerRef = Owner.ToReference(),
                    EmailForAccountingDocuments = EmailForAccountingDocuments.Ensure—leanness(),
                    LegalPersonType = LegalPersonType,
                    PersonResponsibleForDocuments = PersonResponsibleForDocuments,
                    Phone = Phone.Ensure—leanness(),
                    RecipientName = RecipientName.Ensure—leanness(),
                    IsMainProfile = IsMainProfile,
                    Timestamp = Timestamp
                };
        }
    }
}