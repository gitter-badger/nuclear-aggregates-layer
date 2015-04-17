using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
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
    public sealed class CzechLegalPersonViewModel : EntityViewModelBase<LegalPerson>, ILegalNameAspect, IDoesLegalPersonHaveAnyProfilesAspect, ICzechAdapted
    {
        [RequiredLocalized]
        [StringLengthLocalized(256)]
        [Dependency(DependencyType.ReadOnly, "LegalName", "Ext.getDom('Id').value != '0'")]
        public string LegalName { get; set; }

        [Dependency(DependencyType.Disable, "LegalPersonType", "Ext.getDom('Id').value != '0'")]
        public LegalPersonType LegalPersonType { get; set; }

        [StringLengthLocalized(512)]
        public string Comment { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(512)]
        [Dependency(DependencyType.ReadOnly, "LegalAddress", "Ext.getDom('Id').value != '0'")]
        public string LegalAddress { get; set; }

        [DisplayNameLocalized("Dic")]
        [StringLengthLocalized(12)]
        [Dependency(DependencyType.ReadOnly, "Inn", "Ext.getDom('Id').value != '0'")]
        public string Inn { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(8, MinimumLength = 8)]
        [Dependency(DependencyType.ReadOnly, "Ic", "Ext.getDom('Id').value != '0'")]
        public string Ic { get; set; }

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
            Inn = modelDto.Inn;
            Ic = modelDto.Ic;
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
                    Inn = Inn,
                    Ic = Ic,
                    ClientRef = Client.ToReference(),
                    Comment = Comment,
                    OwnerRef = Owner.ToReference(),
                    Timestamp = Timestamp
                };
        }
    }
}