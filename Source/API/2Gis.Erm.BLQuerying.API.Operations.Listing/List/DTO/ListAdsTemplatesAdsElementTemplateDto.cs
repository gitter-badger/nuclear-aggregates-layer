using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListAdsTemplatesAdsElementTemplateDto : IListItemEntityDto<AdsTemplatesAdsElementTemplate>
    {
        public long Id { get; set; }
        public long AdsElementTemplateId { get; set; }
        public long AdsTemplateId { get; set; }
        public string AdsTemplateName { get; set; }
        public string AdsElementTemplateName { get; set; }
        public bool IsDeleted { get; set; }
    }
}