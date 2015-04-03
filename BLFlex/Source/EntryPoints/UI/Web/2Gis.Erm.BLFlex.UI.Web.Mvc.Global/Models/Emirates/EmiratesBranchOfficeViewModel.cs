using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Emirates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Emirates
{
    public sealed class EmiratesBranchOfficeViewModel : EditableIdEntityViewModelBase<BranchOffice>, IEmiratesAdapted
    {
        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string Name { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(512)]
        public string LegalAddress { get; set; }

        [OnlyDigitsLocalized]
        [StringLengthLocalized(10)]
        [RequiredLocalized]
        public string CommercialLicense { get; set; }

        [RequiredLocalized]
        public LookupField BargainType { get; set; }

        [RequiredLocalized]
        public LookupField ContributionType { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (EmiratesBranchOfficeDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            Name = modelDto.Name;
            CommercialLicense = modelDto.CommercialLicense;
            BargainType = LookupField.FromReference(modelDto.BargainTypeRef);
            ContributionType = LookupField.FromReference(modelDto.ContributionTypeRef);
            LegalAddress = modelDto.LegalAddress;
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new EmiratesBranchOfficeDomainEntityDto
                {
                    Id = Id,
                    Name = Name,
                    CommercialLicense = CommercialLicense,
                    BargainTypeRef = BargainType.ToReference(),
                    ContributionTypeRef = ContributionType.ToReference(),
                    LegalAddress = LegalAddress,
                    Timestamp = Timestamp
                };
        }
    }
}