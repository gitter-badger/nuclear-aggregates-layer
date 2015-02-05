using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Ukraine;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Ukraine
{
    public sealed class UkraineBranchOfficeOrganizationUnitViewModel : EditableIdEntityViewModelBase<BranchOfficeOrganizationUnit>, IUkraineAdapted
    {
        [DisplayNameLocalized("BranchOfficeName")]
        [RequiredLocalized]
        [Dependency(DependencyType.ReadOnly, "BranchOffice", "Ext.getCmp('BranchOffice').getValue()!==undefined")]
        public LookupField BranchOffice { get; set; }

        [RequiredLocalized]
        public LookupField OrganizationUnit { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(100)]
        public string ShortLegalName { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string PositionInGenitive { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string PositionInNominative { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string ChiefNameInNominative { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string ChiefNameInGenitive { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string OperatesOnTheBasisInGenitive { get; set; }

        [StringLengthLocalized(256)]
        [RequiredLocalized]
        public string PaymentEssentialElements { get; set; }

        [StringLengthLocalized(512)]
        [RequiredLocalized]
        public string ActualAddress { get; set; }

        [StringLengthLocalized(512)]
        public string PostalAddress { get; set; }

        [StringLengthLocalized(50)]
        public string PhoneNumber { get; set; }

        public bool IsPrimary { get; set; }

        public bool IsPrimaryForRegionalSales { get; set; }

        [Dependency(DependencyType.Hidden, "boinfo", "this.value==''||this.value=='0'")]
        [Dependency(DependencyType.ReadOnly, "BranchOfficeAddlName", "this.value!=''&&this.value!='0'")]
        [Dependency(DependencyType.ReadOnly, "BranchOfficeAddlLegalAddress", "this.value!=''&&this.value!='0'")]
        [Dependency(DependencyType.ReadOnly, "BranchOfficeAddlIpn", "this.value!=''&&this.value!='0'")]
        [Dependency(DependencyType.ReadOnly, "BranchOfficeAddlEgrpou", "this.value!=''&&this.value!='0'")]
        public long? BranchOfficeAddlId { get; set; }

        [DisplayNameLocalized("Name")]
        public string BranchOfficeAddlName { get; set; }

        [DisplayNameLocalized("LegalAddress")]
        [StringLengthLocalized(512)]
        public string BranchOfficeAddlLegalAddress { get; set; }

        [DisplayNameLocalized("Ipn")]
        public string BranchOfficeAddlIpn { get; set; }

        [DisplayNameLocalized("Egrpou")]
        public string BranchOfficeAddlEgrpou { get; set; }

        public string RegistrationCertificate { get; set; }

        [EmailLocalized]
        [StringLengthLocalized(64)]
        public string Email { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (UkraineBranchOfficeOrganizationUnitDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            OrganizationUnit = LookupField.FromReference(modelDto.OrganizationUnitRef);
            BranchOffice = LookupField.FromReference(modelDto.BranchOfficeRef);
            ChiefNameInGenitive = modelDto.ChiefNameInGenitive;
            ChiefNameInNominative = modelDto.ChiefNameInNominative;
            OperatesOnTheBasisInGenitive = modelDto.OperatesOnTheBasisInGenitive;
            IsPrimary = modelDto.IsPrimary;
            IsPrimaryForRegionalSales = modelDto.IsPrimaryForRegionalSales;

            PhoneNumber = modelDto.PhoneNumber;
            PositionInGenitive = modelDto.PositionInGenitive;
            PositionInNominative = modelDto.PositionInNominative;
            ShortLegalName = modelDto.ShortLegalName;
            ActualAddress = modelDto.ActualAddress;
            PostalAddress = modelDto.PostalAddress;
            Email = modelDto.Email;

            BranchOfficeAddlId = modelDto.BranchOfficeAddlId;
            BranchOfficeAddlIpn = modelDto.BranchOfficeAddlIpn;
            BranchOfficeAddlEgrpou = modelDto.BranchOfficeAddlEgrpou;
            BranchOfficeAddlLegalAddress = modelDto.BranchOfficeAddlLegalAddress;
            BranchOfficeAddlName = modelDto.BranchOfficeAddlName;
            PaymentEssentialElements = modelDto.PaymentEssentialElements;

            RegistrationCertificate = modelDto.RegistrationCertificate;
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new UkraineBranchOfficeOrganizationUnitDomainEntityDto
                {
                    Id = Id,
                    OrganizationUnitRef = OrganizationUnit.ToReference(),
                    BranchOfficeRef = BranchOffice.ToReference(),
                    ChiefNameInGenitive = ChiefNameInGenitive,
                    ChiefNameInNominative = ChiefNameInNominative,
                    OperatesOnTheBasisInGenitive = OperatesOnTheBasisInGenitive,
                    IsPrimary = IsPrimary,
                    IsPrimaryForRegionalSales = IsPrimaryForRegionalSales,

                    PhoneNumber = PhoneNumber,
                    PositionInGenitive = PositionInGenitive,
                    PositionInNominative = PositionInNominative,
                    ShortLegalName = ShortLegalName,
                    ActualAddress = ActualAddress,
                    PostalAddress = PostalAddress,
                    Email = Email,

                    BranchOfficeAddlId = BranchOfficeAddlId.Value,
                    BranchOfficeAddlIpn = BranchOfficeAddlIpn,
                    BranchOfficeAddlEgrpou = BranchOfficeAddlEgrpou,
                    BranchOfficeAddlLegalAddress = BranchOfficeAddlLegalAddress,
                    BranchOfficeAddlName = BranchOfficeAddlName,
                    PaymentEssentialElements = PaymentEssentialElements,

                    RegistrationCertificate = RegistrationCertificate,
                    Timestamp = Timestamp
                };
        }
    }
}