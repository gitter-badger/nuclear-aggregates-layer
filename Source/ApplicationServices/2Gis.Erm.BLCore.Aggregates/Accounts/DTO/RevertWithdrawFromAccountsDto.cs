using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Accounts.DTO
{
    public sealed class RevertWithdrawFromAccountsDto
    {
        public Account Account { get; set; }
        public decimal BalanceBeforeRevertWithdrawal { get; set; }
        public IEnumerable<AccountDetail> DebitDetails { get; set; }
    }
}