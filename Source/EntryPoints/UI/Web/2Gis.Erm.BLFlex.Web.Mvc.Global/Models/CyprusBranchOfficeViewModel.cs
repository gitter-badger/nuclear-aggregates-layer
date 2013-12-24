using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Web.Mvc.Utils;
using DoubleGis.Erm.UI.Web.Mvc.Attributes;

namespace DoubleGis.Erm.UI.Web.Mvc.Models
{
    public sealed class CyprusBranchOfficeViewModel : EditableIdEntityViewModelBase<BranchOffice>, ICyprusAdapted
    {
        public long? DgppId { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string Name { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(512)]
        public string LegalAddress { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(11, MinimumLength = 11)]
        [DisplayNameLocalized("Tic")]
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
            var modelDto = (BranchOfficeDomainEntityDto)domainEntityDto;

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