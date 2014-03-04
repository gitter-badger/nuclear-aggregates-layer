using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListAdvertisementDto : IListItemEntityDto<Advertisement>
    {
        public DateTime CreatedOn { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public long AdvertisementTemplateId { get; set; }
        public string AdvertisementTemplateName { get; set; }
        public bool IsSelectedToWhiteList { get; set; }
        public long? FirmId { get; set; }
    }
}