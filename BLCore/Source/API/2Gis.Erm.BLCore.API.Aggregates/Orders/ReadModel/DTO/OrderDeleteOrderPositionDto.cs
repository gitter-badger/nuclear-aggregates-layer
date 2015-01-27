using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel.DTO
{
    public sealed class OrderDeleteOrderPositionDto
    {
        public Order Order { get; set; }
        public OrderPosition OrderPosition { get; set; }
        public bool IsDiscountViaPercentCalculation { get; set; }
    }
}
