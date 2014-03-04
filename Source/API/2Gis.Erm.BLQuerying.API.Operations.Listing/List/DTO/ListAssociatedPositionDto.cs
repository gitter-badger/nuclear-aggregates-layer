using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListAssociatedPositionDto : IListItemEntityDto<AssociatedPosition>
    {
        public long Id { get; set; }
        public long PositionId { get; set; }
        public string PositionName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public long AssociatedPositionsGroupId { get; set; }
    }
}