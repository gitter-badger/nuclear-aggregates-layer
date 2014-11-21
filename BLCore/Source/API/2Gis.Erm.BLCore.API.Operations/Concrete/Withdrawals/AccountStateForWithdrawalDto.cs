using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals
{
    public sealed class AccountStateForWithdrawalDto
    {
        public Lock Lock { get; set; }
        public IEnumerable<LockDetail> Details { get; set; }
        public Account Account { get; set; }
        public decimal LockBalance { get; set; }
        public decimal AccountBalanceBeforeWithdrawal { get; set; }
        public string OrderNumber { get; set; }
    }
}