using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListAssociatedPositionsGroupDto : IListItemEntityDto<AssociatedPositionsGroup>
    {
        public long Id { get; set; }
        public long PricePositionId { get; set; }
        public string Name { get; set; }
        public string PricePositionName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}