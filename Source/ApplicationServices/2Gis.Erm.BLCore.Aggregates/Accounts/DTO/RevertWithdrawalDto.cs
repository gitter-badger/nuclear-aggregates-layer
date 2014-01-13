using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Accounts.DTO
{
    public sealed class RevertWithdrawalDto
    {
        public Account Account { get; set; }
        public AccountDetail DebitAccountDetail { get; set; }
        public decimal AccountBalanceBeforeRevertWithdrawal { get; set; }
        public Lock Lock { get; set; }
        public IEnumerable<LockDetail> LockDetails { get; set; }
        public Order Order { get; set; }
        public decimal AmountAlreadyWithdrawnAfterWithdrawalRevert { get; set; }
        public decimal AmountToWithdrawNextAfterWithdrawalRevert { get; set; }
    }
}