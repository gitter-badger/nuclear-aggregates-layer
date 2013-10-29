using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.UI.Web.Mvc.Attributes;

namespace DoubleGis.Erm.UI.Web.Mvc.Models
{
    public sealed class CyprusBargainTypeViewModel : EditableIdEntityViewModelBase<BargainType>, ICyprusAdapted
    {
        [RequiredLocalized]
        public string Name { get; set; }

        [RequiredLocalized]
        public decimal VatRate { get; set; }

        [StringLengthLocalized(50)]
        public string SyncCode1C { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (BargainTypeDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            Name = modelDto.Name;
            SyncCode1C = modelDto.SyncCode1C;
            VatRate = modelDto.VatRate;
            Timestamp = modelDto.Timestamp;
            IdentityServiceUrl = modelDto.IdentityServiceUrl;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new BargainTypeDomainEntityDto
                {
                    Id = Id,
                    Name = Name,
                    SyncCode1C = SyncCode1C,
                    VatRate = VatRate,
                    Timestamp = Timestamp
                };
        }
    }
}