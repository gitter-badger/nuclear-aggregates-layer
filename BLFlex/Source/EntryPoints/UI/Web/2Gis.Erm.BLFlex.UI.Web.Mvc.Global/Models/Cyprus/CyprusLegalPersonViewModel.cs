using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Cyprus
{
    public sealed class CyprusLegalPersonViewModel : EntityViewModelBase<LegalPerson>, ICyprusAdapted
    {
        [RequiredLocalized]
        [StringLengthLocalized(256)]
        [Dependency(DependencyType.ReadOnly, "LegalName", "Ext.getDom('Id').value != '0'")]
        public string LegalName { get; set; }

        [Dependency(DependencyType.Disable, "LegalPersonType", "Ext.getDom('Id').value != '0'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "PassportNumber", "this.value!='Businessman' && this.value!='NaturalPerson'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "PassportIssuedBy", "this.value!='Businessman' && this.value!='NaturalPerson'")]
        [Dependency(DependencyType.DisableAndHide, "CardNumber", "this.value!='Businessman' && this.value!='NaturalPerson'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "RegistrationAddress", "this.value!='Businessman' && this.value!='NaturalPerson'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "Inn", "this.value=='Businessman' || this.value=='NaturalPerson'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "BusinessmanInn", "this.value!='Businessman'")]
        [Dependency(DependencyType.DisableAndHide, "VAT", "this.value=='NaturalPerson'")]
        [Dependency(DependencyType.NotRequiredDisableHide, "LegalAddress", "this.value=='NaturalPerson'")]
        public LegalPersonType LegalPersonType { get; set; }

        [StringLengthLocalized(512)]
        public string Comment { get; set; }

        [Dependency(DependencyType.ReadOnly, "LegalAddress", "Ext.getDom('Id').value != '0'")]
        [StringLengthLocalized(512)]
        public string LegalAddress { get; set; }

        [DisplayNameLocalized("Tic")]
        [StringLengthLocalized(11, MinimumLength = 11)]
        [Dependency(DependencyType.ReadOnly, "Inn", "Ext.getDom('Id').value != '0'")]
        public string Inn { get; set; }

        [Dependency(DependencyType.ReadOnly, "VAT", "Ext.getDom('Id').value != '0'")]
        [StringLengthLocalized(11, MinimumLength = 11)]
        public string VAT { get; set; }

        [DisplayNameLocalized("Tic")]
        [StringLengthLocalized(11, MinimumLength = 11)]
        [Dependency(DependencyType.ReadOnly, "BusinessmanInn", "Ext.getDom('Id').value != '0'")]
        public string BusinessmanInn { get; set; }

        [StringLengthLocalized(9, MinimumLength = 9)]
        [Dependency(DependencyType.ReadOnly, "PassportNumber", "Ext.getDom('Id').value != '0'")]
        public string PassportNumber { get; set; }

        [StringLengthLocalized(512)]
        public string PassportIssuedBy { get; set; }

        [StringLengthLocalized(512)]
        [DisplayNameLocalized("ResidencyAddress")]
        public string RegistrationAddress { get; set; }

        [StringLengthLocalized(15)]
        [OnlyDigitsLocalized]
        [Dependency(DependencyType.ReadOnly, "CardNumber", "Ext.getDom('Id').value != '0'")]
        public string CardNumber { get; set; }

        [RequiredLocalized]
        public LookupField Client { get; set; }

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
            var modelDto = (LegalPersonDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            LegalName = modelDto.LegalName;
            LegalPersonType = modelDto.LegalPersonTypeEnum;
            LegalAddress = modelDto.LegalAddress;
            Inn = modelDto.LegalPersonTypeEnum == LegalPersonType.LegalPerson ? modelDto.Inn : null;
            VAT = modelDto.VAT;
            BusinessmanInn = modelDto.LegalPersonTypeEnum == LegalPersonType.Businessman ? modelDto.Inn : null;
            PassportNumber = modelDto.PassportNumber;
            PassportIssuedBy = modelDto.PassportIssuedBy;
            RegistrationAddress = modelDto.RegistrationAddress;
            CardNumber = modelDto.CardNumber;
            Client = LookupField.FromReference(modelDto.ClientRef);
            Comment = modelDto.Comment;
            HasProfiles = modelDto.HasProfiles;
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new LegalPersonDomainEntityDto
                {
                    Id = Id,
                    LegalName = LegalName,
                    ShortName = LegalName,
                    LegalPersonTypeEnum = LegalPersonType,
                    LegalAddress = LegalAddress,
                    Inn = LegalPersonType == LegalPersonType.LegalPerson ? Inn : BusinessmanInn,
                    VAT = VAT,
                    PassportNumber = PassportNumber,
                    PassportIssuedBy = PassportIssuedBy,
                    RegistrationAddress = RegistrationAddress,
                    CardNumber = CardNumber,
                    ClientRef = Client.ToReference(),
                    Comment = Comment,
                    OwnerRef = Owner.ToReference(),
                    Timestamp = Timestamp
                };
        }
    }
}