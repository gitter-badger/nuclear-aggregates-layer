using DoubleGis.Erm.BLCore.UI.Metadata.Aspects.Entities;
using DoubleGis.Erm.BLCore.UI.Metadata.Aspects.Entities.Aggregations;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class AssociatedPositionViewModel : EntityViewModelBase<AssociatedPosition>, INewableAndPublishablePriceAspects, IDeletablePriceAspect, IPublishablePriceAspect
    {
        [DisplayNameLocalized("AssociatedPositionsGroupName")]
        public LookupField AssociatedPositionsGroup { get; set; }

        [RequiredLocalized]
        public LookupField Position { get; set; }

        [PresentationLayerProperty]
        public string PositionName { get; set; }

        public LookupField PricePosition { get; set; }

        [RequiredLocalized]
        public ObjectBindingType ObjectBindingType { get; set; }

        public bool PriceIsDeleted { get; set; }

        public bool PriceIsPublished { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var associatedPositionDto = (AssociatedPositionDomainEntityDto)domainEntityDto;

            Id = associatedPositionDto.Id;
            ObjectBindingType = associatedPositionDto.ObjectBindingType;
            Position = LookupField.FromReference(associatedPositionDto.PositionRef);
            AssociatedPositionsGroup = LookupField.FromReference(associatedPositionDto.AssociatedPositionsGroupRef);
            PricePosition = LookupField.FromReference(associatedPositionDto.PricePositionRef);

            PriceIsPublished = associatedPositionDto.PriceIsPublished;
            PriceIsDeleted = associatedPositionDto.PriceIsDeleted;
            Timestamp = associatedPositionDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new AssociatedPositionDomainEntityDto
                {
                    Id = Id,
                    ObjectBindingType = ObjectBindingType,
                    PositionRef = Position.ToReference(),
                    AssociatedPositionsGroupRef = AssociatedPositionsGroup.ToReference(),
                    PricePositionRef = PricePosition.ToReference(),
                    PriceIsPublished = PriceIsPublished,
                    PriceIsDeleted = PriceIsDeleted,
                    Timestamp = Timestamp
                };
        }
    }
}