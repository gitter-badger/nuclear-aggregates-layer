using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
using DoubleGis.Erm.Platform.Model.Aspects;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class AdvertisementTemplateViewModel : EditableIdEntityViewModelBase<AdvertisementTemplate>, IPublishableAspect, INameAspect
    {
        public bool HasActiveAdvertisement { get; set; }

        [RequiredLocalized]
        [StringLengthLocalized(256)]
        public string Name { get; set; }

        [StringLengthLocalized(512)]
        public string Comment { get; set; }

        public bool IsAllowedToWhiteList { get; set; }

        public bool IsAdvertisementRequired { get; set; }
 
        public LookupField DummyAdvertisement { get; set; }

        public bool IsPublished { get; set; }

        public override byte[] Timestamp { get; set; }

        public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
        {
            var advertisementTemplateDto = (AdvertisementTemplateDomainEntityDto)domainEntityDto;
            
            Id = advertisementTemplateDto.Id;
            IsPublished = advertisementTemplateDto.IsPublished;
            IsAllowedToWhiteList = advertisementTemplateDto.IsAllowedToWhiteList;
            HasActiveAdvertisement = advertisementTemplateDto.HasActiveAdvertisement;
            Comment = advertisementTemplateDto.Comment;
            Name = advertisementTemplateDto.Name;
            IsAdvertisementRequired = advertisementTemplateDto.IsAdvertisementRequired;
            Timestamp = advertisementTemplateDto.Timestamp;
            DummyAdvertisement = LookupField.FromReference(advertisementTemplateDto.DummyAdvertisementRef);
        }

        public override IDomainEntityDto TransformToDomainEntityDto()
        {
            return new AdvertisementTemplateDomainEntityDto
                {
                    Id = Id,
                    IsAllowedToWhiteList = IsAllowedToWhiteList,
                    IsAdvertisementRequired = IsAdvertisementRequired,
                    Name = Name,
                    Comment = Comment,
                    Timestamp = Timestamp
                };
        }
    }
}