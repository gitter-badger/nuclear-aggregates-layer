using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class AssociatedPositionsGroupViewModel : EntityViewModelBase<AssociatedPositionsGroup>
    {
        [RequiredLocalized]
        public string Name { get; set; }

        [RequiredLocalized]
        public LookupField PricePosition { get; set; }

        public bool PriceIsDeleted { get; set; }

        public bool PriceIsPublished { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var modelDto = (AssociatedPositionsGroupDomainEntityDto)domainEntityDto;
            Id = modelDto.Id;
            Name = modelDto.Name;
            PriceIsDeleted = modelDto.PriceIsDeleted;
            PriceIsPublished = modelDto.PriceIsPublished;
            PricePosition = LookupField.FromReference(modelDto.PricePositionRef);
            Timestamp = modelDto.Timestamp;
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new AssociatedPositionsGroupDomainEntityDto
                {
                    Id = Id,
                    Name = Name,
                    PriceIsDeleted = PriceIsDeleted,
                    PriceIsPublished = PriceIsPublished,
                    PricePositionRef = PricePosition.ToReference(),
                    Timestamp = Timestamp
                };
        }
    }
}