using System;
using System.Collections.Generic;
using System.Security;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Crosscutting;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;

namespace DoubleGis.Erm.BLCore.Aggregates.Common.Crosscutting
{
    public sealed class AccountDebtsChecker : IAccountDebtsChecker
    {
        private readonly ISecurityServiceFunctionalAccess _functionalAccessService;
        private readonly IAccountReadModel _accountReadModel;

        public AccountDebtsChecker(ISecurityServiceFunctionalAccess functionalAccessService, IAccountReadModel accountReadModel)
        {
            _functionalAccessService = functionalAccessService;
            _accountReadModel = accountReadModel;
        }

        public bool HasDebts(bool bypassValidation,
                             long userCode,
                             Func<IReadOnlyCollection<long>> getTargetAccountsFunc,
                             out string message)
        {
            return HasDebts(bypassValidation, userCode, getTargetAccountsFunc, delegate { }, out message);
        }

        public bool HasDebts(bool bypassValidation,
                             long userCode,
                             Func<IReadOnlyCollection<long>> getTargetAccountsFunc,
                             Action<IReadOnlyCollection<AccountWithDebtInfo>> processErrorsAction,
                             out string message)
        {
            if (bypassValidation)
            {
                if (!_functionalAccessService.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.ProcessAccountsWithDebts, userCode))
                {
                    throw new SecurityException(BLResources.ProcessAccountsWithDebtsOperationIsNotAllowed);
                }
            }
            else
            {
                var accountsWithDebts = _accountReadModel.GetAccountsWithDebts(getTargetAccountsFunc());
                processErrorsAction(accountsWithDebts);

                return AccountsWithDebtsReportGenerator.TryGenerate(accountsWithDebts, out message);
            }

            message = null;
            return false;
        }
    }
}