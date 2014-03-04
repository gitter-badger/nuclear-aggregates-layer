using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListUserTerritoryDto : IListItemEntityDto<UserTerritory>
    {
        public long Id { get; set; }
        public long TerritoryId { get; set; }
        public string TerritoryName { get; set; }
        public bool IsDeleted { get; set; }
        public long UserId { get; set; }
    }
}