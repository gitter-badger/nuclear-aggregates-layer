using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting
{
    [Obsolete("При дальнейшем рефакторинге агрегирующих репозиториев весь функционал переедет в AccountDebtsChecker")]
    public static class DebtsAuditor
    {
        public static void ThrowIfAnyError(IReadOnlyCollection<AccountWithDebtInfo> accountWithDebts)
        {
            if (accountWithDebts == null || !accountWithDebts.Any())
            {
                return;
            }

            var message = new StringBuilder();
            var firstAccount = accountWithDebts.First();

            if (!string.IsNullOrEmpty(firstAccount.ClientName))
            {
                message.AppendFormat(BLResources.CheckForDebtsHelper_ClientTemplate, firstAccount.ClientName);
            }

            foreach (var accountWithDebt in accountWithDebts)
            {
                message.AppendFormat(BLResources.AccountInLegalPersonHasDebts + Environment.NewLine,
                                     accountWithDebt.LegalPersonName,
                                     accountWithDebt.AccountNumber,
                                     accountWithDebt.LockDetailBalance);
            }

            throw new ProcessAccountsWithDebtsException(message.ToString());
        }
    }

    public class AccountWithDebtInfo
    {
        public string ClientName { get; set; }
        public string LegalPersonName { get; set; }
        public long AccountNumber { get; set; }
        public decimal LockDetailBalance { get; set; }
    }
}