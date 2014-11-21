using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListAssociatedPositionsGroupDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public long PricePositionId { get; set; }
        public string Name { get; set; }
        public string PricePositionName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}