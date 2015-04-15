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
            string report;
            if (TryGetReport(accountWithDebts, out report))
            {
                throw new ProcessAccountsWithDebtsException(report);
            }
        }

        public static bool TryGetReport(IReadOnlyCollection<AccountWithDebtInfo> accountWithDebts, out string report)
        {
            if (accountWithDebts == null || !accountWithDebts.Any())
            {
                report = null;
                return false;
            }

            var reportBuilder = new StringBuilder();
            var firstAccount = accountWithDebts.First();

            if (!string.IsNullOrEmpty(firstAccount.ClientName))
            {
                reportBuilder.AppendFormat(BLResources.CheckForDebtsHelper_ClientTemplate, firstAccount.ClientName);
            }

            foreach (var accountWithDebt in accountWithDebts)
            {
                reportBuilder.AppendFormat(BLResources.AccountInLegalPersonHasDebts + Environment.NewLine,
                                           accountWithDebt.LegalPersonName,
                                           accountWithDebt.AccountNumber,
                                           accountWithDebt.LockDetailBalance);
            }


            report = reportBuilder.ToString();
            return true;
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