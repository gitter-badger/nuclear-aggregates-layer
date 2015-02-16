using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class PlatformViewModel : EditableIdEntityViewModelBase<Platform.Model.Entities.Erm.Platform>
    {
        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string Name { get; set; }

        [RequiredLocalized]
        public PositionPlatformPlacementPeriod PlacementPeriod { get; set; }

        [RequiredLocalized]
        public PositionPlatformMinPlacementPeriod MinPlacementPeriod { get; set; }

        [RequiredLocalized]
        public long DgppId { get; set; }

        [YesNoRadio]
        public bool IsSupportedByExport { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (PlatformDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            Name = modelDto.Name;
            DgppId = modelDto.DgppId;
            MinPlacementPeriod = modelDto.MinPlacementPeriodEnum;
            PlacementPeriod = modelDto.PlacementPeriodEnum;
            IsSupportedByExport = modelDto.IsSupportedByExport;
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new PlatformDomainEntityDto
                {
                    Id = Id,
                    Name = Name,
                    DgppId = DgppId,
                    MinPlacementPeriodEnum = MinPlacementPeriod,
                    PlacementPeriodEnum = PlacementPeriod,
                    IsSupportedByExport = IsSupportedByExport,
                    Timestamp = Timestamp
                };
        }
    }
}