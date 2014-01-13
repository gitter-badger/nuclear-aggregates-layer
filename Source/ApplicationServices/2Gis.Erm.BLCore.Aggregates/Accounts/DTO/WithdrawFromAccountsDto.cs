using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Accounts.DTO
{
    public sealed class WithdrawFromAccountsDto
    {
        public string OrderNumber { get; set; }
        public Account Account { get; set; }
        public decimal BalanceBeforeWithdrawal { get; set; }
        public long LockId { get; set; }
        public decimal LockBalance { get; set; }
    }
}