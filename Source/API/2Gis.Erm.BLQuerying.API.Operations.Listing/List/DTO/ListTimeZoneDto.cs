using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListTimeZoneDto : IListItemEntityDto<TimeZone>
    {
        public long Id { get; set; }
        public string TimeZoneId { get; set; }
        public string Description { get; set; }
    }
}