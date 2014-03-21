using System;

using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListAdvertisementDto : IOperationSpecificEntityDto
    {
        public DateTime CreatedOn { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public long AdvertisementTemplateId { get; set; }
        public string AdvertisementTemplateName { get; set; }
        public bool IsSelectedToWhiteList { get; set; }
        public long? FirmId { get; set; }
        public bool IsDeleted { get; set; }
    }
}