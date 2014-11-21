using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;

namespace DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting
{
    internal class AccountWithDebtInfo
    {
        public string ClientName { get; set; }
        public string LegalPersonName { get; set; }
        public long AccountNumber { get; set; }
        public decimal LockDetailBalance { get; set; }
    }

    internal static class CheckForDebtsHelper
    {
        public static string CollectErrors(IEnumerable<AccountWithDebtInfo> accountWithDebts)
        {
            // ReSharper disable PossibleMultipleEnumeration
            if (accountWithDebts == null || !accountWithDebts.Any())
            {
                return string.Empty;
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

            return message.ToString();
            // ReSharper restore PossibleMultipleEnumeration
        }
    }
}