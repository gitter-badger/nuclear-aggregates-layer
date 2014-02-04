using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListAdvertisementTemplateDto : IListItemEntityDto<AdvertisementTemplate>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsPublished { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
