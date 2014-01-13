using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models
{
    public sealed class CzechBargainTypeViewModel : EditableIdEntityViewModelBase<BargainType>, ICzechAdapted
    {
        [RequiredLocalized]
        public string Name { get; set; }

        [RequiredLocalized]
        public decimal VatRate { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (BargainTypeDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            Name = modelDto.Name;
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
                    VatRate = VatRate,
                    Timestamp = Timestamp
                };
        }
    }
}