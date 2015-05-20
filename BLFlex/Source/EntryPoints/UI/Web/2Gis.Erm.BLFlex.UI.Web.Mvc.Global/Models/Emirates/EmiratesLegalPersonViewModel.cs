using System;
using System.Globalization;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Emirates;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Aspects.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Emirates
{
    public sealed class EmiratesLegalPersonViewModel : EntityViewModelBase<LegalPerson>, ILegalNameAspect, IDoesLegalPersonHaveAnyProfilesAspect, IEmiratesAdapted
    {
        [RequiredLocalized]
        [StringLengthLocalized(256)]
        [Dependency(DependencyType.ReadOnly, "LegalName", "Ext.getDom('Id').value != '0'")]
        public string LegalName { get; set; }

        [Dependency(DependencyType.ReadOnly, "LegalPersonType", "true")]
        public string LegalPersonType { get; set; }

        [RequiredLocalized]
        public LookupField Client { get; set; }

        [OnlyDigitsLocalized]
        [StringLengthLocalized(10)]
        [RequiredLocalized]
        [Dependency(DependencyType.ReadOnly, "CommercialLicense", "Ext.getDom('Id').value != '0'")]
        public string CommercialLicense { get; set; }

        [RequiredLocalized]
        [Dependency(DependencyType.ReadOnly, "CommercialLicenseBeginDate", "Ext.getDom('Id').value != '0'")]
        public DateTime? CommercialLicenseBeginDate { get; set; }

        [RequiredLocalized]
        [GreaterOrEqualThan("CommercialLicenseBeginDate", ErrorMessageResourceType = typeof(BLResources), ErrorMessageResourceName = "EndDateMustntBeLessThanBeginDate")]
        [Dependency(DependencyType.ReadOnly, "CommercialLicenseEndDate", "Ext.getDom('Id').value != '0'")]
        public DateTime? CommercialLicenseEndDate { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(512)]
        [Dependency(DependencyType.ReadOnly, "LegalAddress", "Ext.getDom('Id').value != '0'")]
        public string LegalAddress { get; set; }
        
        [StringLengthLocalized(512)]
        public string Comment { get; set; }

        public override byte[] Timestamp { get; set; }

        [RequiredLocalized]
        public override LookupField Owner { get; set; }

        public override bool IsSecurityRoot
        {
            get { return true; }
        }

        public bool HasProfiles { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (EmiratesLegalPersonDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            CommercialLicense = modelDto.CommercialLicense;
            CommercialLicenseBeginDate = modelDto.CommercialLicenseBeginDate;
            CommercialLicenseEndDate = modelDto.CommercialLicenseEndDate;
            LegalName = modelDto.LegalName;
            LegalPersonType = modelDto.LegalPersonTypeEnum.ToStringLocalized(EnumResources.ResourceManager, CultureInfo.CurrentCulture);
            LegalAddress = modelDto.LegalAddress;
            Client = LookupField.FromReference(modelDto.ClientRef);
            Comment = modelDto.Comment;
            HasProfiles = modelDto.HasProfiles;
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new EmiratesLegalPersonDomainEntityDto
                {
                    Id = Id,
                    LegalName = LegalName.EnsureСleanness(),
                    LegalAddress = LegalAddress.EnsureСleanness(),
                    ClientRef = Client.ToReference(),
                    Comment = Comment,
                    OwnerRef = Owner.ToReference(),
                    Timestamp = Timestamp,
                    CommercialLicense = CommercialLicense.EnsureСleanness(),
                    CommercialLicenseBeginDate = CommercialLicenseBeginDate.Value,
                    CommercialLicenseEndDate = CommercialLicenseEndDate.Value,
                    LegalPersonTypeEnum = Platform.Model.Entities.Enums.LegalPersonType.LegalPerson
                };
        }
    }
}