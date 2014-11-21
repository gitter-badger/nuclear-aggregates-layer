using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListAdsTemplatesAdsElementTemplateDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public long AdsElementTemplateId { get; set; }
        public long AdsTemplateId { get; set; }
        public string AdsTemplateName { get; set; }
        public string AdsElementTemplateName { get; set; }
        public bool IsDeleted { get; set; }
    }
}