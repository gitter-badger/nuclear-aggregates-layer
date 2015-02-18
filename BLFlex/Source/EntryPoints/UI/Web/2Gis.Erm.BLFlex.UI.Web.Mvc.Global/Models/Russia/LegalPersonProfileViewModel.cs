using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Russia;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Russia
{
    public sealed class LegalPersonProfileViewModel : EntityViewModelBase<LegalPersonProfile>, IRussiaAdapted
    {
        [SanitizedString]
        [RequiredLocalized]
        public string Name { get; set; }

        [Dependency(DependencyType.NotRequiredDisableHide, "PositionInNominative", "this.value=='NaturalPerson'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "PositionInGenitive", "this.value=='NaturalPerson'")]
        [Dependency(DependencyType.Required, "PositionInGenitive", "this.value!='NaturalPerson'")]
        [Dependency(DependencyType.Required, "PositionInNominative", "this.value!='NaturalPerson'")]
        [Dependency(DependencyType.Required, "OperatesOnTheBasisInGenitive", "this.value!='NaturalPerson'")]
        public LegalPersonType LegalPersonType { get; set; }

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
        public string ChiefFullNameInNominative { get; set; }
        
        [SanitizedString]
        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string ChiefNameInGenitive { get; set; }

        [Dependency(DependencyType.NotRequiredDisableHide, "CertificateNumber", "this.value!='Certificate'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "CertificateDate", "this.value!='Certificate'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "WarrantyNumber", "this.value!='Warranty'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "WarrantyEndDate", "this.value!='Warranty'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "WarrantyBeginDate", "this.value!='Warranty'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "BargainNumber", "this.value!='Bargain'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "BargainEndDate", "this.value!='Bargain'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "BargainBeginDate", "this.value!='Bargain'")]
        public OperatesOnTheBasisType OperatesOnTheBasisInGenitive { get; set; }

        [SanitizedString]
        [StringLengthLocalized(512, MinimumLength = 0)]
        public string DocumentsDeliveryAddress { get; set; }

        [SanitizedString]
        [StringLengthLocalized(50, MinimumLength = 0)]
        public string CertificateNumber { get; set; }

        public DateTime? CertificateDate { get; set; }

        [SanitizedString]
        [StringLengthLocalized(50)]
        public string WarrantyNumber { get; set; }

        public DateTime? WarrantyBeginDate { get; set; }

        public DateTime? WarrantyEndDate { get; set; }

        [SanitizedString]
        [StringLengthLocalized(50)]
        public string BargainNumber { get; set; }

        public DateTime? BargainBeginDate { get; set; }

        public DateTime? BargainEndDate { get; set; }

        [SanitizedString]
        [StringLengthLocalized(512)]
        [RequiredLocalized]
        public string PostAddress { get; set; }

        [SanitizedString]
        [StringLengthLocalized(256)]
        public string RecipientName { get; set; }

        [Dependency(DependencyType.Required, "DocumentsDeliveryAddress", "this.value == 'DeliveryByManager'")]
        public DocumentsDeliveryMethod DocumentsDeliveryMethod { get; set; }

        [SanitizedString]
        [EmailLocalized]
        [StringLengthLocalized(64)]
        public string EmailForAccountingDocuments { get; set; }

        [SanitizedString]
        [EmailLocalized]
        [StringLengthLocalized(64)]
        public string Email { get; set; }

        [SanitizedString]
        [StringLengthLocalized(256)]
        [RequiredLocalized]
        public string PersonResponsibleForDocuments { get; set; }

        [SanitizedString]
        [DisplayNameLocalized("ContactPhone")]
        [StringLengthLocalized(50)]
        public string Phone { get; set; }

        [SanitizedString]
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

        public string[] DisabledDocuments { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (RussiaLegalPersonProfileDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            Name = modelDto.Name;
            Email = modelDto.AdditionalEmail;
            ChiefNameInGenitive = modelDto.ChiefNameInGenitive;
            ChiefNameInNominative = modelDto.ChiefNameInNominative;
            ChiefFullNameInNominative = modelDto.ChiefFullNameInNominative;
            DocumentsDeliveryAddress = modelDto.DocumentsDeliveryAddress;
            DocumentsDeliveryMethod = modelDto.DocumentsDeliveryMethod;
            LegalPerson = LookupField.FromReference(modelDto.LegalPersonRef);
            PositionInNominative = modelDto.PositionInNominative;
            PositionInGenitive = modelDto.PositionInGenitive;
            OperatesOnTheBasisInGenitive = modelDto.OperatesOnTheBasisInGenitive;
            CertificateDate = modelDto.CertificateDate;
            CertificateNumber = modelDto.CertificateNumber;
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
            return new RussiaLegalPersonProfileDomainEntityDto
                {
                    Id = Id,
                    Name = Name.EnsureСleanness(),
                    AdditionalEmail = Email.EnsureСleanness(),
                    ChiefNameInGenitive = ChiefNameInGenitive.EnsureСleanness(),
                    ChiefNameInNominative = ChiefNameInNominative.EnsureСleanness(),
                    ChiefFullNameInNominative = ChiefFullNameInNominative.EnsureСleanness(),
                    DocumentsDeliveryAddress = DocumentsDeliveryAddress.EnsureСleanness(),
                    DocumentsDeliveryMethod = DocumentsDeliveryMethod,
                    LegalPersonRef = LegalPerson.ToReference(),
                    PositionInNominative = PositionInNominative.EnsureСleanness(),
                    PositionInGenitive = PositionInGenitive.EnsureСleanness(),
                    OperatesOnTheBasisInGenitive = OperatesOnTheBasisInGenitive,
                    CertificateDate = CertificateDate,
                    CertificateNumber = CertificateNumber.EnsureСleanness(),
                    BargainBeginDate = BargainBeginDate,
                    BargainEndDate = BargainEndDate,
                    BargainNumber = BargainNumber.EnsureСleanness(),
                    WarrantyNumber = WarrantyNumber.EnsureСleanness(),
                    WarrantyBeginDate = WarrantyBeginDate,
                    WarrantyEndDate = WarrantyEndDate,
                    PostAddress = PostAddress.EnsureСleanness(),
                    OwnerRef = Owner.ToReference(),
                    EmailForAccountingDocuments = EmailForAccountingDocuments.EnsureСleanness(),
                    LegalPersonType = LegalPersonType,
                    PaymentEssentialElements = PaymentEssentialElements.EnsureСleanness(),
                    PersonResponsibleForDocuments = PersonResponsibleForDocuments.EnsureСleanness(),
                    Phone = Phone.EnsureСleanness(),
                    RecipientName = RecipientName.EnsureСleanness(),
                    IsMainProfile = IsMainProfile,
                    Timestamp = Timestamp
                };
        }
    }
}