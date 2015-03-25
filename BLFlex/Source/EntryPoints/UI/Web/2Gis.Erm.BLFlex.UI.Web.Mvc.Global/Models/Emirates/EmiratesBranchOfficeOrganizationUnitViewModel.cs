using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Emirates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Emirates
{
    public sealed class EmiratesBranchOfficeOrganizationUnitViewModel : EditableIdEntityViewModelBase<BranchOfficeOrganizationUnit>, IEmiratesAdapted
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
        public string ApplicationCityName { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string PositionInNominative { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string ChiefNameInNominative { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string PaymentEssentialElements { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(512)]
        public string ActualAddress { get; set; }

        [StringLengthLocalized(512)]
        public string PostalAddress { get; set; }

        [StringLengthLocalized(50)]
        public string PhoneNumber { get; set; }

        public string Fax { get; set; }

        public bool IsPrimary { get; set; }

        public bool IsPrimaryForRegionalSales { get; set; }

        [Dependency(DependencyType.Hidden, "boinfo", "this.value==''||this.value=='0'")]
        [Dependency(DependencyType.ReadOnly, "BranchOfficeAddlName", "this.value!=''&&this.value!='0'")]
        [Dependency(DependencyType.ReadOnly, "BranchOfficeAddlLegalAddress", "this.value!=''&&this.value!='0'")]
        [Dependency(DependencyType.ReadOnly, "BranchOfficeAddlCommercialLicense", "this.value!=''&&this.value!='0'")]
        public long? BranchOfficeAddlId { get; set; }

        [DisplayNameLocalized("Name")]
        public string BranchOfficeAddlName { get; set; }

        [DisplayNameLocalized("LegalAddress")]
        [StringLengthLocalized(512)]
        public string BranchOfficeAddlLegalAddress { get; set; }

        [DisplayNameLocalized("CommercialLicense")]
        public string BranchOfficeAddlCommercialLicense { get; set; }

        [EmailLocalized]
        [StringLengthLocalized(64)]
        public string Email { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (EmiratesBranchOfficeOrganizationUnitDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            OrganizationUnit = LookupField.FromReference(modelDto.OrganizationUnitRef);
            BranchOffice = LookupField.FromReference(modelDto.BranchOfficeRef);
            ChiefNameInNominative = modelDto.ChiefNameInNominative;
            IsPrimary = modelDto.IsPrimary;
            IsPrimaryForRegionalSales = modelDto.IsPrimaryForRegionalSales;

            ApplicationCityName = modelDto.ApplicationCityName;
            PhoneNumber = modelDto.PhoneNumber;
            Fax = modelDto.Fax; 
            PositionInNominative = modelDto.PositionInNominative;
            ShortLegalName = modelDto.ShortLegalName;
            ActualAddress = modelDto.ActualAddress;
            PostalAddress = modelDto.PostalAddress;
            Email = modelDto.Email;

            BranchOfficeAddlId = modelDto.BranchOfficeAddlId;
            BranchOfficeAddlCommercialLicense = modelDto.BranchOfficeAddlCommercialLicense;
            BranchOfficeAddlLegalAddress = modelDto.BranchOfficeAddlLegalAddress;
            BranchOfficeAddlName = modelDto.BranchOfficeAddlName;
            PaymentEssentialElements = modelDto.PaymentEssentialElements;

            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new EmiratesBranchOfficeOrganizationUnitDomainEntityDto
                {
                    Id = Id,
                    OrganizationUnitRef = OrganizationUnit.ToReference(),
                    BranchOfficeRef = BranchOffice.ToReference(),
                    ChiefNameInNominative = ChiefNameInNominative,
                    IsPrimary = IsPrimary,
                    IsPrimaryForRegionalSales = IsPrimaryForRegionalSales,

                    PhoneNumber = PhoneNumber,
                    Fax = Fax,
                    PositionInNominative = PositionInNominative,
                    ShortLegalName = ShortLegalName,
                    ActualAddress = ActualAddress,
                    PostalAddress = PostalAddress,
                    Email = Email,

                    BranchOfficeAddlId = BranchOfficeAddlId.Value,
                    BranchOfficeAddlCommercialLicense = BranchOfficeAddlCommercialLicense,
                    
                    BranchOfficeAddlLegalAddress = BranchOfficeAddlLegalAddress,
                    BranchOfficeAddlName = BranchOfficeAddlName,
                    PaymentEssentialElements = PaymentEssentialElements,
                    Timestamp = Timestamp
                };
        }
    }
}