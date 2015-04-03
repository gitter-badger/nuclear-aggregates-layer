using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class DenialReasonViewModel : EntityViewModelBase<DenialReason>
    {
        [RequiredLocalized]
        [StringLengthLocalized(256)]
        [DisplayNameLocalized("DenialReasonName")]
        public string Name { get; set; }

        [DisplayNameLocalized("DenialReasonDescription")]
        public string Description { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(256)]
        [DisplayNameLocalized("DenialReasonProofLink")]
        [UrlLocalized]
        public string ProofLink { get; set; }

        [RequiredLocalized]
        [DisplayNameLocalized("DenialReasonType")]
        public DenialReasonType Type { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (DenialReasonDomainEntityDto)domainEntityDto;
            Id = modelDto.Id;

            Name = modelDto.Name;
            Description = modelDto.Description;
            ProofLink = modelDto.ProofLink;
            Type = modelDto.Type;
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new DenialReasonDomainEntityDto
                {
                    Id = Id,
                    Name = Name,
                    Description = Description,
                    ProofLink = ProofLink,
                    Type = Type,
                    Timestamp = Timestamp
                };
        }
    }
}