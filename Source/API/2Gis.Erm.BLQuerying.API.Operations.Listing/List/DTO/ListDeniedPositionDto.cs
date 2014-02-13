using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListDeniedPositionDto : IListItemEntityDto<DeniedPosition>
    {
        public long Id { get; set; }
        public long PositionDeniedId { get; set; }
        public string PositionDeniedName { get; set; }
   }
}