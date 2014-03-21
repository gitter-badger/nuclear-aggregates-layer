using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListAssociatedPositionDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public long PositionId { get; set; }
        public string PositionName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public long AssociatedPositionsGroupId { get; set; }
    }
}