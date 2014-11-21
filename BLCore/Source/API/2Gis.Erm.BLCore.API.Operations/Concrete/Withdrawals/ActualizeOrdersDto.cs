using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals
{
    public sealed class ActualizeOrdersDto
    {
        public Order Order { get; set; }
        public decimal AmountAlreadyWithdrawn { get; set; }
        public decimal AmountToWithdrawNext { get; set; }
    }
}