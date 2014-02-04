using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListAdvertisementElementTemplateDto : IListItemEntityDto<AdvertisementElementTemplate>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string RestrictionType { get; set; }
    }
}