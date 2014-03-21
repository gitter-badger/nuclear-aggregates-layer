using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListDeniedPositionDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public long PositionDeniedId { get; set; }
        public string PositionDeniedName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
   }
}