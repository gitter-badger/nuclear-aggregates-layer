using DoubleGis.Erm.BLCore.UI.Metadata.Aspects;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Kazakhstan;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Kazakhstan
{
    public sealed class KazakhstanBranchOfficeViewModel : EditableIdEntityViewModelBase<BranchOffice>, INameAspect, IKazakhstanAdapted
    {
        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string Name { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(512)]
        public string LegalAddress { get; set; }

        [StringLengthLocalized(12, MinimumLength = 12)]
        public string Bin { get; set; }

        [RequiredLocalized]
        public LookupField BargainType { get; set; }

        [RequiredLocalized]
        public LookupField ContributionType { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (KazakhstanBranchOfficeDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            Name = modelDto.Name;
            Bin = modelDto.Bin;
            BargainType = LookupField.FromReference(modelDto.BargainTypeRef);
            ContributionType = LookupField.FromReference(modelDto.ContributionTypeRef);
            LegalAddress = modelDto.LegalAddress;
            Timestamp = modelDto.Timestamp;
            IdentityServiceUrl = modelDto.IdentityServiceUrl;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new KazakhstanBranchOfficeDomainEntityDto
                {
                    Id = Id,
                    Name = Name,
                    Bin = Bin, 
                    BargainTypeRef = BargainType.ToReference(),
                    ContributionTypeRef = ContributionType.ToReference(),
                    LegalAddress = LegalAddress,
                    Timestamp = Timestamp
                };
        }
    }
}