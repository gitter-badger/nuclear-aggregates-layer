using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting
{
    public class AccountWithDebtInfo
    {
        public string ClientName { get; set; }
        public string LegalPersonName { get; set; }
        public long AccountNumber { get; set; }
        public decimal LockDetailBalance { get; set; }
    }

    public static class CheckForDebtsHelper
    {
        public static void ThrowIfAnyError(IReadOnlyCollection<AccountWithDebtInfo> accountWithDebts)
        {
            // ReSharper disable PossibleMultipleEnumeration
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

            if (message.Length != 0)
            {
                throw new ProcessAccountsWithDebtsException(message.ToString());
            }
        }
    }
}