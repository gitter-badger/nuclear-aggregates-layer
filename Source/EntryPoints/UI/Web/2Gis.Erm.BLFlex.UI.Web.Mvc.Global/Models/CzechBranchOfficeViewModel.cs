using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models
{
    public sealed class CzechBranchOfficeViewModel : EditableIdEntityViewModelBase<BranchOffice>, ICzechAdapted
    {
        public long? DgppId { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string Name { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(512)]
        public string LegalAddress { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(12)]
        [DisplayNameLocalized("Dic")]
        public string Inn { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(8, MinimumLength = 8)]
        public string Ic { get; set; }

        [RequiredLocalized]
        public LookupField BargainType { get; set; }

        [RequiredLocalized]
        public LookupField ContributionType { get; set; }

        public string Annotation { get; set; }

        public string UsnNotificationText { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (BranchOfficeDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            DgppId = modelDto.DgppId;
            Name = modelDto.Name;
            Inn = modelDto.Inn;
            Ic = modelDto.Ic;
            Annotation = modelDto.Annotation;
            BargainType = LookupField.FromReference(modelDto.BargainTypeRef);
            ContributionType = LookupField.FromReference(modelDto.ContributionTypeRef);
            LegalAddress = modelDto.LegalAddress;
            UsnNotificationText = modelDto.UsnNotificationText;
            Timestamp = modelDto.Timestamp;
            IdentityServiceUrl = modelDto.IdentityServiceUrl;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new BranchOfficeDomainEntityDto
                {
                    Id = Id,
                    DgppId = DgppId,
                    Name = Name,
                    Inn = Inn,
                    Ic = Ic,
                    Annotation = Annotation,
                    BargainTypeRef = BargainType.ToReference(),
                    ContributionTypeRef = ContributionType.ToReference(),
                    LegalAddress = LegalAddress,
                    UsnNotificationText = UsnNotificationText,
                    Timestamp = Timestamp
                };
        }
    }
}