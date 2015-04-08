using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Aspects.Entities;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class DeniedPositionViewModel : EntityViewModelBase<DeniedPosition>, IPublishablePriceAspect
    {
        [PresentationLayerProperty]
        public long PriceId { get; set; }

        [RequiredLocalized]
        public LookupField Position { get; set; }

        [RequiredLocalized]
        public LookupField PositionDenied { get; set; }

        [RequiredLocalized]
        public ObjectBindingType ObjectBindingType { get; set; }

        public bool PriceIsPublished { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (DeniedPositionDomainEntityDto)domainEntityDto;

            Id = modelDto.Id;
            Position = LookupField.FromReference(modelDto.PositionRef);
            PositionDenied = LookupField.FromReference(modelDto.PositionDeniedRef);
            PriceId = modelDto.PriceRef.Id.Value;
            ObjectBindingType = modelDto.ObjectBindingType;
            PriceIsPublished = modelDto.PriceIsPublished;
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new DeniedPositionDomainEntityDto
                {
                    Id = Id,
                    PositionRef = Position.ToReference(),
                    PositionDeniedRef = PositionDenied.ToReference(),
                    PriceRef = new EntityReference(PriceId),
                    ObjectBindingType = ObjectBindingType,
                    PriceIsPublished = PriceIsPublished,
                    Timestamp = Timestamp
                };
        }
    }
}