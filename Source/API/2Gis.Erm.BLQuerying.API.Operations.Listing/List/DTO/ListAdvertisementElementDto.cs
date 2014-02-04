using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListAdvertisementElementDto : IListItemEntityDto<AdvertisementElement>
    {
        public long Id { get; set; }
        public long AdvertisementElementTemplateId { get; set; }
        public string AdvertisementElementTemplateName { get; set; }
        public bool IsRequired { get; set; }
        public string RestrictionType { get; set; }
    }
}