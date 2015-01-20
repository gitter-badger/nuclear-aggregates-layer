using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel.DTO
{
    public sealed class OrderAmountToWithdrawInfo
    {
        public Order Order { get; set; }
        public decimal AmountToWithdraw { get; set; }
    }
}
