using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Accounts.DTO
{
    public sealed class WithdrawalDto
    {
        public Account Account { get; set; }
        public decimal AccountBalanceBeforeWithdrawal { get; set; }
        public Lock Lock { get; set; }
        public IEnumerable<LockDetail> LockDetails { get; set; }
        public decimal CalculatedLockBalance { get; set; }
        public Order Order { get; set; }
        public decimal AmountAlreadyWithdrawnAfterWithdrawal { get; set; }
        public decimal AmountToWithdrawNextAfterWithdrawal { get; set; }
    }
}