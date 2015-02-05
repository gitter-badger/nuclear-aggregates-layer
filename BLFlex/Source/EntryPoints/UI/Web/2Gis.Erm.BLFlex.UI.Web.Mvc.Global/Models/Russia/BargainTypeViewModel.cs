using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Russia
{
    public sealed class BargainTypeViewModel : EditableIdEntityViewModelBase<BargainType>, IRussiaAdapted
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