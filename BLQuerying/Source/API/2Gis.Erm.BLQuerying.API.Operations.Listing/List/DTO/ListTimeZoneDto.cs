using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListTimeZoneDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public string TimeZoneId { get; set; }
    }
}
