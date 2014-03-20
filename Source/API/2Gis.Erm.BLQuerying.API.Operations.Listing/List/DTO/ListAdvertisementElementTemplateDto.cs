using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListAdvertisementElementTemplateDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public AdvertisementElementRestrictionType RestrictionTypeEnum { get; set; }
        public string RestrictionType { get; set; }
        public bool IsDeleted { get; set; }
    }
}