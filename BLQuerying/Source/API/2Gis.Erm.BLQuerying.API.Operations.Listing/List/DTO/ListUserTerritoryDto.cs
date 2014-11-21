using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListUserTerritoryDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public long TerritoryId { get; set; }
        public string TerritoryName { get; set; }
        public long UserId { get; set; }
        public bool TerritoryIsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}