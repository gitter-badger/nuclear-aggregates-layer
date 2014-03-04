using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListOrderPositionDto : IListItemEntityDto<OrderPosition>
    {
        public long Id { get; set; }
        public string PositionName { get; set; }
        public long OrderId { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal Amount { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountSum { get; set; }
        public decimal PayablePlan { get; set; }
        public bool IsDeleted { get; set; }
    }
}