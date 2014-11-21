using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListAdvertisementElementDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public long AdvertisementElementTemplateId { get; set; }
        public string AdvertisementElementTemplateName { get; set; }
        public bool IsRequired { get; set; }
        public string RestrictionType { get; set; }
        public long AdvertisementId { get; set; }
    }
}