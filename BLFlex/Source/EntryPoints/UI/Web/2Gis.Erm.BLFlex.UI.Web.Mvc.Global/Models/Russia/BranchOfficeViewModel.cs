using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Russia;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Russia
{
    public sealed class BranchOfficeViewModel : EditableIdEntityViewModelBase<BranchOffice>, INameAspect, IRussiaAdapted
    {
        public long? DgppId { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string Name { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(512)]
        public string LegalAddress { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(12, MinimumLength = 10)]
        [OnlyDigitsLocalized]
        public string Inn { get; set; }

        [RequiredLocalized]
        public LookupField BargainType { get; set; }

        [RequiredLocalized]
        public LookupField ContributionType { get; set; }

        public string Annotation { get; set; }

        public string UsnNotificationText { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (RussiaBranchOfficeDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            DgppId = modelDto.DgppId;
            Name = modelDto.Name;
            Inn = modelDto.Inn;
            Annotation = modelDto.Annotation;
            BargainType = LookupField.FromReference(modelDto.BargainTypeRef);
            ContributionType = LookupField.FromReference(modelDto.ContributionTypeRef);
            LegalAddress = modelDto.LegalAddress;
            UsnNotificationText = modelDto.UsnNotificationText;
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new RussiaBranchOfficeDomainEntityDto
                {
                    Id = Id,
                    DgppId = DgppId,
                    Name = Name,
                    Inn = Inn,
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