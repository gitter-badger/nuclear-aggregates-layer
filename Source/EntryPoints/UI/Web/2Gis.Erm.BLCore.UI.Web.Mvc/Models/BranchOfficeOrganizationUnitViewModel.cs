using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Models
{
    public sealed class BranchOfficeOrganizationUnitViewModel : EditableIdEntityViewModelBase<BranchOfficeOrganizationUnit>, IRussiaAdapted
    {
        [DisplayNameLocalized("BranchOfficeName")]
        [RequiredLocalized]
        [Dependency(DependencyType.ReadOnly, "BranchOffice", "Ext.getCmp('BranchOffice').getValue()!==undefined")]
        public LookupField BranchOffice { get; set; }

        [RequiredLocalized]
        public LookupField OrganizationUnit { get; set; }

        [RequiredLocalized]
        public string ShortLegalName { get; set; }

        [RequiredLocalized]
        public string PositionInGenitive { get; set; }

        [RequiredLocalized]
        public string PositionInNominative { get; set; }

        [RequiredLocalized]
        public string ChiefNameInNominative { get; set; }

        [RequiredLocalized]
        public string ChiefNameInGenitive { get; set; }

        [RequiredLocalized]
        public string OperatesOnTheBasisInGenitive { get; set; }

        [StringLengthLocalized(9, MinimumLength = 9)]
        [OnlyDigitsLocalized]
        public string Kpp { get; set; }

        [StringLengthLocalized(256)]
        public string PaymentEssentialElements { get; set; }

        [StringLengthLocalized(512)]
        [RequiredLocalized]
        public string ActualAddress { get; set; }

        [StringLengthLocalized(512)]
        public string PostalAddress { get; set; }

        public string PhoneNumber { get; set; }

        [RequiredLocalized]
        public bool IsPrimary { get; set; }

        public bool IsPrimaryForRegionalSales { get; set; }

        [Dependency(DependencyType.Hidden, "boinfo", "this.value==''||this.value=='0'")]
        [Dependency(DependencyType.ReadOnly, "BranchOfficeAddlName", "this.value!=''&&this.value!='0'")]
        [Dependency(DependencyType.ReadOnly, "BranchOfficeAddlLegalAddress", "this.value!=''&&this.value!='0'")]
        [Dependency(DependencyType.ReadOnly, "BranchOfficeAddlInn", "this.value!=''&&this.value!='0'")]
        public long? BranchOfficeAddlId { get; set; }

        [DisplayNameLocalized("Name")]
        public string BranchOfficeAddlName { get; set; }

        [DisplayNameLocalized("LegalAddress")]
        [StringLengthLocalized(512)]
        public string BranchOfficeAddlLegalAddress { get; set; }

        [DisplayNameLocalized("Inn")]
        public string BranchOfficeAddlInn { get; set; }

        public string SyncCode1C { get; set; }

        public string RegistrationCertificate { get; set; }

        [EmailLocalized]
        [StringLengthLocalized(64)]
        public string Email { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (BranchOfficeOrganizationUnitDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            OrganizationUnit = LookupField.FromReference(modelDto.OrganizationUnitRef);
            BranchOffice = LookupField.FromReference(modelDto.BranchOfficeRef);
            ChiefNameInGenitive = modelDto.ChiefNameInGenitive;
            ChiefNameInNominative = modelDto.ChiefNameInNominative;
            IsPrimary = modelDto.IsPrimary;
            IsPrimaryForRegionalSales = modelDto.IsPrimaryForRegionalSales;
            OperatesOnTheBasisInGenitive = modelDto.OperatesOnTheBasisInGenitive;

            Kpp = modelDto.Kpp;
            PhoneNumber = modelDto.PhoneNumber;
            PositionInGenitive = modelDto.PositionInGenitive;
            PositionInNominative = modelDto.PositionInNominative;
            ShortLegalName = modelDto.ShortLegalName;
            ActualAddress = modelDto.ActualAddress;
            PostalAddress = modelDto.PostalAddress;
            Email = modelDto.Email;

            BranchOfficeAddlId = modelDto.BranchOfficeAddlId;
            BranchOfficeAddlInn = modelDto.BranchOfficeAddlInn;
            BranchOfficeAddlLegalAddress = modelDto.BranchOfficeAddlLegalAddress;
            BranchOfficeAddlName = modelDto.BranchOfficeAddlName;
            PaymentEssentialElements = modelDto.PaymentEssentialElements;

            SyncCode1C = modelDto.SyncCode1C;
            RegistrationCertificate = modelDto.RegistrationCertificate;
            Timestamp = modelDto.Timestamp;
            IdentityServiceUrl = modelDto.IdentityServiceUrl;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new BranchOfficeOrganizationUnitDomainEntityDto
                {
                    Id = Id,
                    OrganizationUnitRef = OrganizationUnit.ToReference(),
                    BranchOfficeRef = BranchOffice.ToReference(),
                    ChiefNameInGenitive = ChiefNameInGenitive,
                    ChiefNameInNominative = ChiefNameInNominative,
                    IsPrimary = IsPrimary,
                    IsPrimaryForRegionalSales = IsPrimaryForRegionalSales,
                    OperatesOnTheBasisInGenitive = OperatesOnTheBasisInGenitive,

                    Kpp = Kpp,
                    PhoneNumber = PhoneNumber,
                    PositionInGenitive = PositionInGenitive,
                    PositionInNominative = PositionInNominative,
                    ShortLegalName = ShortLegalName,
                    ActualAddress = ActualAddress,
                    PostalAddress = PostalAddress,
                    Email = Email,

                    BranchOfficeAddlId = BranchOfficeAddlId.Value,
                    BranchOfficeAddlInn = BranchOfficeAddlInn,
                    BranchOfficeAddlLegalAddress = BranchOfficeAddlLegalAddress,
                    BranchOfficeAddlName = BranchOfficeAddlName,
                    PaymentEssentialElements = PaymentEssentialElements,

                    SyncCode1C = SyncCode1C,
                    RegistrationCertificate = RegistrationCertificate,
                    Timestamp = Timestamp
                };
        }
    }
}