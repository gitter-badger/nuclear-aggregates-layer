using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals
{
    public sealed class AccountStateForRevertingWithdrawalDto
    {
        public Lock Lock { get; set; }
        public IEnumerable<LockDetail> LockDetails { get; set; }
        public Account Account { get; set; }
        public AccountDetail DebitAccountDetail { get; set; }
        public decimal AccountBalanceBeforeRevertingWithdrawal { get; set; }
    }
}