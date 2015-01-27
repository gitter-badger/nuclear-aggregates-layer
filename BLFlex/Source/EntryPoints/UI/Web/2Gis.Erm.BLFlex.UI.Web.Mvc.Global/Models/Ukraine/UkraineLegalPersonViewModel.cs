using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Ukraine;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Aspects.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Ukraine
{
    public sealed class UkraineLegalPersonViewModel : EntityViewModelBase<LegalPerson>, ILegalNameAspect, IDoesLegalPersonHaveAnyProfilesAspect, ILegalPersonEgrpouHolder, IUkraineAdapted
    {
        [RequiredLocalized]
        [StringLengthLocalized(256)]
        [Dependency(DependencyType.ReadOnly, "LegalName", "Ext.getDom('Id').value != '0'")]
        public string LegalName { get; set; }

        [Dependency(DependencyType.NotRequiredDisableHide, "Egrpou", "this.value=='Businessman'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "BusinessmanEgrpou", "this.value!='Businessman'")]
        [Dependency(DependencyType.Disable, "LegalPersonType", "Ext.getDom('Id').value != '0'")]
        public LegalPersonType LegalPersonType { get; set; }

        [RequiredLocalized]
        public LookupField Client { get; set; }

        [OnlyDigitsLocalized]
        [StringLengthLocalized(12, MinimumLength = 10)]
        [Dependency(DependencyType.ReadOnly, "Ipn", "Ext.getDom('Id').value != '0'")]
        public string Ipn { get; set; }

        [OnlyDigitsLocalized]
        [StringLengthLocalized(8)]
        [Dependency(DependencyType.ReadOnly, "Egrpou", "Ext.getDom('Id').value != '0'")]
        public string Egrpou { get; set; }

        [OnlyDigitsLocalized]
        [StringLengthLocalized(10)]
        [DisplayNameLocalized("Egrpou")]
        [Dependency(DependencyType.ReadOnly, "BusinessmanEgrpou", "Ext.getDom('Id').value != '0'")]
        public string BusinessmanEgrpou { get; set; }

        [RequiredLocalized]
        [Dependency(DependencyType.Required, "Ipn", "this.value.toLowerCase() == 'withvat'")]
        [ExcludeZeroValue]
        [Dependency(DependencyType.Disable, "TaxationType", "Ext.getDom('Id').value != '0'")]
        public TaxationType TaxationType { get; set; }

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
            var modelDto = (UkraineLegalPersonDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            Ipn = modelDto.Inn;
            LegalName = modelDto.LegalName;
            LegalPersonType = modelDto.LegalPersonTypeEnum;
            LegalAddress = modelDto.LegalAddress;
            TaxationType = modelDto.TaxationType;
            Client = LookupField.FromReference(modelDto.ClientRef);
            Comment = modelDto.Comment;
            HasProfiles = modelDto.HasProfiles;
            Timestamp = modelDto.Timestamp;

            this.SetEgrpou(modelDto.Egrpou);
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new UkraineLegalPersonDomainEntityDto
                {
                    Id = Id,
                    LegalName = LegalName,
                    LegalPersonTypeEnum = LegalPersonType,
                    LegalAddress = LegalAddress,
                    ClientRef = Client.ToReference(),
                    Comment = Comment,
                    OwnerRef = Owner.ToReference(),
                    Timestamp = Timestamp,
                    Inn = Ipn,
                    Egrpou = this.GetEgrpou(),
                    TaxationType = TaxationType
                };
        }
    }
}